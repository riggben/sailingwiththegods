//Paul Reichling
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PetteiaGameController : MonoBehaviour
{
	public Vector2Int rewardAmts;

	[Header("Game Pieces")]
	public List<PetteiaPlayerPiece> playerPieces;
	public PetteiaEnemyAI enemyAI;

	public AudioSource moveSound;

	[Header("Board Positions")]
	public PetteiaBoardPosition[] squaresRow0 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow1 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow2 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow3 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow4 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow5 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow6 = new PetteiaBoardPosition[8];
	public PetteiaBoardPosition[] squaresRow7 = new PetteiaBoardPosition[8];
	[HideInInspector] public int[,] positions = new int[8, 8];

	[Header("UI")]
	public MiniGameInfoScreen mgScreen;
	public TavernaMiniGameDialog playerBarks;
	public TavernaEnemyDialog enemyBarks;
	public Sprite gameIcon;
	public float barkChance = 0.25f;

	[Header("Text")]
	public string introText;
	[TextArea(2, 30)]
	public string instructions;
	public string winText;
	public string loseText;
	
	[TextArea(1, 8)]
	public string debugPiecePositions;
	
	private bool playerTurn;
	private bool gameOver = false;
	
	void Start() 
	{
		mgScreen.gameObject.SetActive(true);
		string text = introText + "\n\n" + instructions + "\n\n" + "flavor";
		mgScreen.DisplayText("Petteia", "Taverna game", text, gameIcon, MiniGameInfoScreen.MiniGame.TavernaStart);
		enemyAI = GetComponent<PetteiaEnemyAI>();
		playerTurn = true;

		//Turns the various rows of board squares into one 2D array
		//Since public 2D arrays don't show in the inspector, we have to do it this way
		for (int i = 0; i < 8; i++) {
			BoardSquares[0, i] = squaresRow0[i];
			BoardSquares[1, i] = squaresRow1[i];
			BoardSquares[2, i] = squaresRow2[i];
			BoardSquares[3, i] = squaresRow3[i];
			BoardSquares[4, i] = squaresRow4[i];
			BoardSquares[5, i] = squaresRow5[i];
			BoardSquares[6, i] = squaresRow6[i];
			BoardSquares[7, i] = squaresRow7[i];
		}
		InitalStateSetup();

		//Player goes first, so their pieces are highlighted and the enemy's are not
		HighlightPlayerPieces(true);
		enemyAI.ToggleEnemyHighlight(false);
	}

	public void EnableAllPlayerPieces() 
	{
		foreach (PetteiaPlayerPiece p in playerPieces) 
		{
			p.StartGame();
		}
	}

	public void PauseMinigame() 
	{
		mgScreen.gameObject.SetActive(true);
		Time.timeScale = 0;
		mgScreen.DisplayText("Petteia", "Taverna game", instructions, gameIcon, MiniGameInfoScreen.MiniGame.TavernaPause);
	}

	public void UnpauseMinigame() 
	{
		mgScreen.gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	public void ExitMinigame() 
	{
		TavernaController.BackToTavernaMenu();
	}

	public void RestartMinigame() 
	{
		TavernaController.ReloadTavernaGame("Petteia");
	}

	/// <summary>
	/// Checks for captures and game over, and switches the control between the player/AI
	/// </summary>
	public void SwitchTurn() 
	{
		UpdateDebugText();
		if (playerTurn) {
			//Switching from player turn to enemy turn
			CheckCapture();
			CheckPlayerBlocked();
			playerTurn = false;
			enemyAI.CheckPieces();
			StartCoroutine(CheckGameOver());
			
			HighlightPlayerPieces(false);
			enemyAI.ToggleEnemyHighlight(true);

			enemyAI.StartEnemyTurn();
		}
		else 
		{
			//Switching from enemy turn to player turn
			CheckCapture();
			CheckPlayerBlocked();
			playerTurn = true;
			enemyAI.CheckPieces();
			StartCoroutine(CheckGameOver());

			HighlightPlayerPieces(true);
			enemyAI.ToggleEnemyHighlight(false);
		}
	}

	/// <summary>
	/// Toggles the highlight on all player pieces
	/// </summary>
	/// <param name="toggle"></param>
	public void HighlightPlayerPieces(bool toggle) 
	{
		foreach (PetteiaPlayerPiece p in playerPieces) {
			p.ToggleHighlight(toggle);
		}
	}

	/// <summary>
	/// Plays the movement sound at a random pitch
	/// </summary>
	public void PlayMoveSound() 
	{
		moveSound.pitch = Random.Range(0.7f, 1.1f);
		moveSound.Play();
	}

	private void InitalStateSetup() 
	{
		//1 - enemy
		//2 - player

		//We could roll these into one since enemy and player should always have the same number of pieces
		//But while testing, I made one or the other start with fewer so it would be easier to finish
		for (int i = 0; i < enemyAI.pieces.Count; i++) 
		{
			positions[enemyAI.pieces[i].pieceStartPos.x, enemyAI.pieces[i].pieceStartPos.y] = 1;
			BoardSquares[enemyAI.pieces[i].pieceStartPos.x, enemyAI.pieces[i].pieceStartPos.y].occupied = true;
		}

		for (int i = 0; i < playerPieces.Count; i++) 
		{
			positions[playerPieces[i].pieceStartPos.x, playerPieces[i].pieceStartPos.y] = 2;
			BoardSquares[playerPieces[i].pieceStartPos.x, playerPieces[i].pieceStartPos.y].occupied = true;
		}
	}

	/// <summary>
	/// Checks if any pieces have been captured
	/// </summary>
	public void CheckCapture() 
	{
		for (int y = 0; y < 8; y++) 
		{
			for (int x = 0; x < 8; x++) 
			{
				//Checks for vertical captures - not possible on the top or bottom rows
				if (x != 0 && x != 7) 
				{
					if (positions[x, y] == 1
						&& positions[x + 1, y] == 2
						&& positions[x - 1, y] == 2) 
					{
						Debug.Log("Enemy captured by player vertically");
						CapturePiece(x, y);
					}
					
					if (positions[x, y] == 2
						&& positions[x + 1, y] == 1
						&& positions[x - 1, y] == 1) 
					{
						Debug.Log("Player captured by enemy vertically");
						CapturePiece(x, y);
					}
					
				}
				//Checks for horizontal captures - not possible on the left- or rightmost columns
				if (y != 0 && y != 7) 
				{
					if (positions[x, y] == 1
						&& positions[x, y + 1] == 2
						&& positions[x, y - 1] == 2) 
					{
						Debug.Log("Enemy captured by player horizontally");
						CapturePiece(x, y);
					}
					
					if (positions[x, y] == 2
						&& positions[x, y + 1] == 1
						&& positions[x, y - 1] == 1) 
					{
						Debug.Log("Player captured by enemy horizontally");
						CapturePiece(x, y);
					}
				}
			}
		}
	}

	/// <summary>
	/// Checks if either the player or opponent is down to 1 piece left
	/// </summary>
	public IEnumerator CheckGameOver() 
	{
		//Debug.Log($"Players: {playerPieces.Count} | Enemies: {enemyAI.pieces.Count}");
		yield return null;
		enemyAI.CheckPieces();
		yield return null;

		//Player win
		if (enemyAI.pieces.Count <= 1) 
		{
			mgScreen.gameObject.SetActive(true);

			//Minimum pieces left to win is 2, maximum is all 8
			//So we need to map [rewardAmt.x rewardAmt.y] to [2, 8]
			float oldRange = 6f;
			float newRange = rewardAmts.y - rewardAmts.x;			
			int reward = Mathf.CeilToInt(((playerPieces.Count - 2) * (newRange * 1.0f) / oldRange) + rewardAmts.x);

			string text = winText + "\n\n" + $"For your victory, you win {reward} food and water!";

			if (Globals.GameVars != null) 
			{
				Globals.GameVars.playerShipVariables.ship.AddToFoodAndWater(reward);
			}

			mgScreen.DisplayText("Petteia Victory", "Taverna Game", text, gameIcon, MiniGameInfoScreen.MiniGame.TavernaEnd);
			gameOver = true;
		}

		//Player loss
		if (playerPieces.Count <= 1) {
			mgScreen.gameObject.SetActive(true);
			string text = loseText + "\n\n" + "Although you have lost this round, you can always find a willing opponent to try again!";
			mgScreen.DisplayText("Petteia Loss", "Taverna Game", text, gameIcon, MiniGameInfoScreen.MiniGame.TavernaEnd);
			gameOver = true;
		}
	}
	
	public void BlockingGameOver(bool playerBlocked) 
	{
		if (playerBlocked) {
			//player can't move, has therefore lost
			Debug.Log("Player is blocked in");
			mgScreen.gameObject.SetActive(true);
			string text = "Lost because of blocking" + "\n\n" + "Although you have lost this round, you can always find a willing opponent to try again!";
			mgScreen.DisplayText("Petteia Loss", "Taverna Game", text, gameIcon, MiniGameInfoScreen.MiniGame.TavernaEnd);
			gameOver = true;
		}
		else {
			//enemy can't move, player has therefore won
			//Won't have special text, so can just trick the game into thinking the player won normally
			Debug.Log("Enemy is blocked in");
			enemyAI.pieces.Clear();
			StartCoroutine(CheckGameOver());
		}
	}

	private void CheckPlayerBlocked() 
	{
		for (int i = 0; i < playerPieces.Count; i++) 
		{
			List<PetteiaBoardPosition> validMoves = playerPieces[i].PopulateValidMovesList(playerPieces[i].pieceStartPos);

			//If any one player piece can still move, you're not blocked
			//We check if it's more than 1 because the square the piece is currently on is always counted
			if (validMoves.Count > 1) 
			{
				return;
			}
		}

		BlockingGameOver(true);
	}

	/// <summary>
	/// Captures the piece located at [i, j]
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	private void CapturePiece(int i, int j) 
	{
		BoardSquares[i, j].DestroyPiece();
		int enemyDone = 0;
		enemyDone = enemyAI.CheckPieces();
		int tries = 0;

		//This is some really weird code that might not be necessary
		//Originally, I had this as a coroutine with yield return enemyAI.CheckPieces()
		//And that worked great except when the player intentionally moved into a capture
		//I have absolutely no idea what the problem was, but changing it to not be a coroutine fixed it?
		while (enemyDone != 1 && tries < 1000) 
		{
			Debug.Log("wait...");
		}
		if (tries >= 200) 
		{
			Debug.Log("Waited too long for the enemy to check its pieces");
		}
		
		if (Random.Range(0f, 1f) < barkChance) 
		{
			if (Random.Range(0f, 1f) > 0.5f) 
			{
				//player captures enemy - player brags
				if (positions[i, j] == 1) {
					playerBarks.DisplayBragging();
				}
				//enemy captures player - player insults
				else if (positions[i, j] == 2) {
					playerBarks.DisplayInsult();
				}
			}
			else {
				//player captures enemy - enemy insults
				if (positions[i, j] == 1) {
					enemyBarks.DisplayInsult();
				}
				//enemy captures player - enemy brags
				else if (positions[i, j] == 2) {
					enemyBarks.DisplayBragging();
				}
			}

		}

		positions[i, j] = 0;
		UpdateDebugText();
	}

	/// <summary>
	/// Prints the board pieces to debugPiecePositions
	/// Player pieces are O, enemy pieces are X, and empty spaces are -
	/// </summary>
	private void UpdateDebugText() 
	{
		string s = "";
		for (int i = 0; i < 8; i++) 
		{
			for (int j = 0; j < 8; j++) 
			{
				switch (positions[i, j]) 
				{
					case 0:
						s += "-";
						break;
					case 1:
						s += "X";
						break;
					case 2:
						s += "O";
						break;
				}
				s += " ";
			}
			s += "\n";
		}
		debugPiecePositions = s;
	}

	/// <summary>
	/// Moves a piece tagged for player/enemy from oldPos to newPos
	/// </summary>
	/// <param name="oldPos"></param>
	/// <param name="newPos"></param>
	/// <param name="tag"></param>
	public void MovePiece(Vector2Int oldPos, Vector2Int newPos, string tag) 
	{
		positions[oldPos.x, oldPos.y] = 0;
		positions[newPos.x, newPos.y] = tag == "PetteiaW" ? 2 : 1;
	}

	
	private Vector2Int PosToArray(float y, float x) 
	{
		return new Vector2Int(Mathf.RoundToInt((y + 3.25f) / -6.25f), Mathf.RoundToInt((x - 3) / 6.25f));
		//converts the real world cordinates of the pieces to the value of the array that stores where the pieces are
	}

    public PetteiaBoardPosition[,] BoardSquares { get; } = new PetteiaBoardPosition[8, 8];

	public bool GameOver 
	{
		get { return gameOver; }
	}

	public bool PlayerTurn {
		get { return playerTurn; }
	}
}
