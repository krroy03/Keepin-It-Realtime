using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetectClick : MonoBehaviour
{
	
		public bool clickPiece = false;
		public bool clickSquare = false;
		public bool attacking = false;
		public bool playerTurn; // true is white, false is black
		public bool whiteCheck = false;
		public bool whiteCheckmate = false;
		public bool blackCheck = false;
		public bool blackCheckmate = false;
		public bool gameOver = false;
		public MoveChessPiece[] whitePieces;
		public MoveChessPiece[] blackPieces;
		public MoveChessPiece curPiece;
		public MoveChessPiece attackedPiece;
		public Material activeState;
		public Material white;
		public Material black;
		public int whitePieceCount = 16;
		public int blackPieceCount = 16;
		private float diff = 1.2f;
		public int row;
		public int col;
		public ChessGameTree tree;
		public MasterNetworking server;
		// variables for whether single player or multi player


		private bool multiplayer = true;
		public bool singlePlayer = false;

		// variables for multiplayer networking 
		public bool moveValidated = false;
		public bool sentRequest = false;
		public class Cell
		{
				public bool hasChessPiece;
				public int row;
				public int col;

				// 0 if no piece on it
				// 1 for pawn, 2 for rook, 3 for knight, 4 for bishop, 5 for king , 6 for queen // black values
				// 7 for pawn,  8 for rook, 9 for knight, 10 for bishop, 11 for king , 12 for queen// white values
				public int piece; 

		}
		// represents the chess board, only 1 instance of this grid in this class
		public Cell[,] grid = new Cell[8, 8] ;

		// Use this for initialization
		void Awake ()
		{
				setUpGame (false);
		server = GameObject.FindGameObjectWithTag ("Network").GetComponent<MasterNetworking> ();
		}
		
		void setUpGame (bool ai)
		{
				// set up the squares on the board
				playerTurn = true;
				attacking = false;
				clickPiece = false;
				clickSquare = false;
				whiteCheck = false;
				whiteCheckmate = false;
				blackCheck = false;
				blackCheckmate = false;
				gameOver = false;
				whitePieceCount = 16;
				blackPieceCount = 16;
				multiplayer = true;
				singlePlayer = ai;

				// set up the grid
		
				for (int i=0; i < 8; i++) {
						for (int j=0; j < 8; j++) {
								var cur = new Cell ();
								cur.row = i;
								cur.col = j; 
								if (i <= 1 || i >= 6)
										cur.hasChessPiece = true;
								if (i == 1)
										cur.piece = 1; // black pawn
				else if (i == 6) 
										cur.piece = 7; // white pawn
				else if (i == 0) {  // all other black pieces
										if (j <= 4)
												cur.piece = j + 2;
										else
												switch (j) {
												case 5:
														cur.piece = 4;
														break;
												case 6:
														cur.piece = 3;
														break;
												case 7:
														cur.piece = 2;
														break;
												}
					
								} else if (i == 7) {  // all other white pieces
										if (j <= 4)
												cur.piece = j + 8;
										else
												switch (j) {
												case 5:
														cur.piece = 10;
														break;
												case 6:
														cur.piece = 9;
														break;
												case 7:
														cur.piece = 8;
														break;
												}
					
								} else {
										cur.piece = 0; // empty squares
										cur.hasChessPiece = false;
								}
								grid [i, j] = cur;
						}
				}
		}
		
		void OnGUI ()
		{
				if (gameOver) {
						if (singlePlayer) {

			
								if (whiteCheck) {
										GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "White in Check");
								} else if (blackCheck) {
										GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Black in Check");
								} else if (whiteCheckmate) {
										GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "White in Checkmate");

										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Sorry.. You Lost...");
								} else if (blackCheckmate) {
										GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Black in Checkmate");

										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Congrats! You Won!");
								}
			

						}
				}
		}

		public void debugGrid (Cell[,] grid)
		{
				for (int i=0; i < 8; i++) {
						for (int j=0; j < 8; j++) {
								Debug.Log (grid [i, j].piece);
						}

				}

		}

		// Update is called once per frame
		void Update ()
		{		
				// visitor doesn't do any of the work, only server does
				// he does not need to validate moves, simply makes them
				// the server validates the move
		
				if (!gameOver && multiplayer && server.visitor && server.myTurn) {
		
						if (!clickPiece || !clickSquare) // keep checking for clicks only if we need to, else wait to get back move
								CheckClick (); 
						if (clickPiece && clickSquare && !sentRequest) {// send move to server to be validated 
								server.validateMove ();
								sentRequest = true;

						}
						// then, we test if we made a proper click in the chessserver script. 
						// then send that move to server, which if correct, sets moveValidated to true
						// it also sets the curpiece, row and col so we can make the move.
						if (server.gotResponse) {  // got a response from server
								if (moveValidated) {
										Debug.Log ("visitor got back validated move");
										// variables all set already, so simply make move and reset values
										makeMove ();
										resetMove (true);
										server.myTurn = !server.myTurn;
										moveValidated = false;
										sentRequest = false;
								} else {
										print ("invalid Move");
										sentRequest = false;
										resetMove (false);
								}
								server.gotResponse = false; // set to false either way
						}
						
				} else if (!gameOver && multiplayer && server.visitor && !server.myTurn) { // visitor waiting for host to make a move
						if (server.gotResponse) { // definitely a valid move, so simply make the move
								makeMove ();
								playerTurn = !playerTurn; // this is all we need to do
								server.myTurn = !server.myTurn;
								server.gotResponse = false;
								Debug.Log ("vistor got host move");
						}
				} else if (!gameOver && multiplayer && !server.visitor && !server.myTurn && server.gotRequest) { // host trying to validate move he got from visitor
						bool madeMove = false;
						if (curPiece.isValidMove (playerTurn, row, col)) {
								if (checkPathBetweenMoves (grid, curPiece.row, curPiece.col, row, col, playerTurn, curPiece)) {
					
										server.gotRequest = false;
										server.sendMove (true, false); // server needs to send this move back to visitor
										makeMove ();
										// reset values now
										resetMove (true);
										madeMove = true;
										Debug.Log ("host validated move and sent it");
										
										server.myTurn = !server.myTurn;
								}
						}
						if (!madeMove) {
								print ("invalid Move");
								// start over again
								resetMove (false);
				
								server.gotRequest = false;
								server.sendMove (false, false);
								
				
						}
				}
				// host needs to make a move to send to visitor
		else if (!gameOver && multiplayer && !server.visitor && server.myTurn) {
						CheckClick ();
						bool madeMove = false;
						if (clickPiece && clickSquare && !gameOver) {
								if (curPiece.isValidMove (playerTurn, row, col)) {
										print ("passed first move validity check");
										if (checkPathBetweenMoves (grid, curPiece.row, curPiece.col, row, col, playerTurn, curPiece)) {
												Debug.Log ("host made a move");
												if (server.networked) {// only send move if player is playing on a network
														
														server.sendMove (false, true); 
														server.myTurn = !server.myTurn;
												}
												makeMove ();

												// reset values now
												madeMove = true;

												if (madeMove) 
														resetMove (true);
												if (singlePlayer) 
														multiplayer = false;
											
										}
								}
								if (!madeMove) {
										print ("invalid Move");
										// start over again
										resetMove (false);

								}
						}
				} else if (!gameOver && !clickPiece && !clickSquare && !multiplayer && !playerTurn) { // ai turn to move, or blacks turn
						
						var curNode = tree.makeCurNode (row, col, false);
						// we now have a root node for our game tree. 
						// so now just run alphabeta search from this node
						var bestNode = tree.RunAlphaBeta (curNode, tree.maxSearchDepth, int.MinValue, int.MaxValue, false);

						// we now have the best node to get to, just need to extract the move that takes us there. 

						var bestMove = bestNode.move;
						var oldCol = bestMove.fromCol;
						var oldRow = bestMove.fromRow;
						// find piece to move
						foreach (MoveChessPiece piece in blackPieces) {
								if (piece.row == oldRow && piece.col == oldCol) {
										curPiece = piece;
								}
						}
						row = bestMove.curRow;
						col = bestMove.curCol;
			
						print ("moving " + curPiece.name + " from col : " + oldCol + " , row : " + oldRow + " , to col : " + row + " , row : ");

						if (grid [row, col].hasChessPiece) 
								attacking = true;
						foreach (MoveChessPiece piece in whitePieces) {
								if (piece.row == row && piece.col == col) {
										attackedPiece = piece;
								}
						}

						// we have done all the setting up, we can make the move now
						makeMove ();
						resetMove (true);
						multiplayer = true;
				}
		
		}
		
		public void resetGame (bool ai)
		{
				setUpGame (ai);
				foreach (MoveChessPiece piece in whitePieces) {
						piece.setUpPiece (true);
				}
				foreach (MoveChessPiece piece in blackPieces) {
						piece.setUpPiece (true);
				}
		}

		void makeMove ()
		{
		
				// save the row and col values, overwrite old ones
				grid [curPiece.row, curPiece.col].hasChessPiece = false;
				grid [curPiece.row, curPiece.col].piece = 0;
				curPiece.savedRow = curPiece.row;
				curPiece.savedCol = curPiece.col;
				curPiece.row = row;
				curPiece.col = col;
				grid [row, col].hasChessPiece = true;
				grid [row, col].piece = curPiece.value;
		
				if (attacking) {
						if (playerTurn) {
								blackPieceCount -= 1;
						} else {
								whitePieceCount -= 1;
						}
						attackedPiece.RemovePiece ();
						if (attackedPiece.CompareTag ("King")) {
								gameOver = true;
								if (attackedPiece.value < 7) 
										blackCheckmate = true;
								else
										whiteCheckmate = true;
						}
				}
				curPiece.MovePiece ();

				// after player makes move, check if he has checked or checkmated the other player
				bool check1 = isCheckmate (grid, !playerTurn);
				bool checkmate = true;
				if (check1) { // check if any possible legal move that takes you out of this quasi-checkmate
						// get all legal moves
						List <ChessGameTree.Node> children = tree.FindChildren (tree.makeCurNode (this.row, this.col, !playerTurn));
						foreach (ChessGameTree.Node child in children) { // if any one of those nodes returns a non quasi-checkmate instance, this is not checkmate yet.
								
								if (!isCheckmate (child.move.after.grid, !playerTurn)) {
										debugGrid (child.move.after.grid);
										checkmate = false;
									
										print ("found non-terminal node");
								}
						}
				}
				if (playerTurn && check1 && checkmate) { // if player is white and check2 is true, then white has checkmated black
						blackCheckmate = checkmate;
				} else if (!playerTurn && check1 && checkmate) {
						whiteCheckmate = checkmate;
				}
				if (check1 && checkmate) 
						gameOver = true;
				else if (isCheck (grid, !playerTurn)) {
						if (playerTurn) // white checked black
								blackCheck = true;
						else // black checked white
								whiteCheck = true;
				} else {
						if (playerTurn && whiteCheck) {
								// assume player moved out of check pos
								whiteCheck = false;
						} else if (!playerTurn && blackCheck) {
								blackCheck = false;
						}
				}
		}


		// we only run this function before moving to the next players turn, player can't use this, we use this for verification purposes only
		void UndoLastMove (MoveChessPiece curPiece, int row, int col)
		{
	
				// vacate its new spot
				grid [curPiece.row, curPiece.col].hasChessPiece = false;
				grid [curPiece.row, curPiece.col].piece = 0;
				curPiece.row = row;
				curPiece.col = col;

				// place piece back to old spot
				grid [row, col].hasChessPiece = true;
				grid [row, col].piece = curPiece.value;

		
				if (attacking) {
						if (playerTurn) {
								blackPieceCount += 1;
						} else {
								whitePieceCount += 1;
						}
						attackedPiece.UndoRemovePiece ();
				}
				curPiece.MovePiece ();
				if (gameOver) {
						gameOver = !gameOver;
						whiteCheckmate = false;
						blackCheckmate = false;

				}

		}

		void resetMove (bool valid)
		{
		
				clickPiece = false;
				clickSquare = false;
				attacking = false;
				if (playerTurn)
						curPiece.renderer.material = white;
				else 
						curPiece.renderer.material = black;
				
				if (valid) 
						playerTurn = !playerTurn;
		}
		/*
	 * Detects if the player clicked a chess piece or square
	 * Cast a ray at that position, and check if it hits a chess piece. 
	 * If it hits a chess piece, mark that chess piece as active if it is the valid turn for that color
	 * returns 1 if player clicked a chess piece, returns 2 if hits square
	 * If clicked a square, mark that square as active if a chess piece is marked active
	 * returns 0 if no mouse click yet or if click something that is not a chess piece or square
	 * */
		int CheckClick ()
		{
				// Make sure the user pressed the mouse down
				if (!Input.GetMouseButtonDown (0))
						return 0;
		
				// We need to actually hit an object
				RaycastHit hit;
				var clickPos = Input.mousePosition; 
				bool click = Physics.Raycast (Camera.main.ScreenPointToRay (clickPos), out hit, 100);
				if (click) { // if clicked on on object
						if (hit.rigidbody == null) {
								return 0;
						}
						if (hit.collider.CompareTag ("ChessSquare")) { // hit a square 
								
								if (clickPiece) { // if have active chess piece, we mark this square as active, so as to move chess piece there
										
										clickSquare = true;
										row = getRowInfo (hit);
										col = getColInfo (hit);

								}
								// hit a square
								return 2;
						}

						// reaches here if we hit a chess piece instead
						if (!hit.collider.CompareTag ("ChessBoard")) {
								if (clickPiece) { // if another chess piece is already marked active
										clickChessPieceTwice (hit);
										return 1;
								} else { // no chess piece marked as ready to move yet
										clickChessPieceOnce (hit);
										return 1;
								}
						} else 
								return 0;
				} else { // ray didn't hit any objects
						return 0;
				}
		}

		void clickChessPieceTwice (RaycastHit hit)
		{
				if (curPiece.piecePos.position.Equals (hit.transform.position) && hit.collider.CompareTag (curPiece.tag)) { 
						// if new  clicked chess piece is the same as current active
						// simply return true without doing anything, we need to click a square still to move the chess piece onto
						return;
				} else { 
						// new clicked piece is different from current active
						// if same color piece, then we simply change current active
						// if enemy piece, we move piece to attack that piece. 
						var changed = false;
						// so now, by chess rules, we will only move chess pieces to empty square or square where enemy piece is. 
						if (playerTurn) {
								foreach (MoveChessPiece piece in whitePieces) {
										if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
												curPiece.renderer.material = white;
												curPiece = piece; 
												curPiece.renderer.material = activeState;
												changed = true;
												print ("changed active white piece");
										}
								}
								if (!changed) {
										foreach (MoveChessPiece piece in blackPieces) {
												if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
														attackedPiece = piece; 
														row = getRowInfo (hit);
														col = getColInfo (hit);
														attacking = true; 
														clickSquare = true;
														print ("attacking");
												}
										}
								}
						} else {
								foreach (MoveChessPiece piece in blackPieces) {
										if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
												curPiece.renderer.material = black;
												curPiece = piece; 
												curPiece.renderer.material = activeState; 
												changed = true;
												print ("changed active black piece");
										}
								}

								if (!changed) {
										foreach (MoveChessPiece piece in whitePieces) {
												if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
														attackedPiece = piece; 
														row = getRowInfo (hit);
														col = getColInfo (hit);
														attacking = true; 
														clickSquare = true;
														print ("attacking");
												}
										}
								}

						
								
						}
						return;
				}
		}

		void clickChessPieceOnce (RaycastHit hit)
		{
				// if no chess piece is marked active
				// simply set curPiece to whatever piece we clicked on, set clickPiece to true, and return true
		
				var misclick = true;
				if (playerTurn) {
						foreach (MoveChessPiece piece in whitePieces) {
								if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
										curPiece = piece; 
										curPiece.renderer.material = activeState;
										misclick = false;
										print ("clicked white piece");
								}
						}
				} else {
						foreach (MoveChessPiece piece in blackPieces) {
								if (piece.transform.position.Equals (hit.transform.position) && hit.collider.CompareTag (piece.tag)) {
										curPiece = piece; 
										curPiece.renderer.material = activeState;
										misclick = false;
										print ("clicked black piece");
								}
						}
				}
				if (misclick) // white player clicked on black piece or vice versa
						clickPiece = false;
				else 
						clickPiece = true;
				return;
		}

		int getRowInfo (RaycastHit hit)
		{
				var piece = hit.collider;
				var pos = hit.transform.position;
				if (piece.CompareTag ("Pawn")) {
						return (int)Mathf.RoundToInt ((pos.x + 8.4f) / diff);
				} else if (piece.CompareTag ("Rook")) {
						return (int)Mathf.RoundToInt ((pos.x + 8.4f) / diff);
				} else if (piece.CompareTag ("Knight")) {
						return (int)Mathf.RoundToInt ((pos.x + 7.2f) / diff);
				} else if (piece.CompareTag ("Bishop")) {
						return (int)Mathf.RoundToInt ((pos.x + 6.0f) / diff);
				} else if (piece.CompareTag ("Queen")) {
						return (int)Mathf.RoundToInt ((pos.x + 4.8f) / diff);
				} else if (piece.CompareTag ("King")) {
						return (int)Mathf.RoundToInt ((pos.x + 3.6f) / diff);
				} else if (piece.CompareTag ("ChessSquare")) {
						return (int)Mathf.RoundToInt ((pos.x + 4.2f) / diff);
				}
		
				return -4;
		}
	
		int getColInfo (RaycastHit hit)
		{
				var piece = hit.collider;
				var pos = hit.transform.position;
				if (piece.CompareTag ("Pawn")) {
						return (int)Mathf.RoundToInt ((pos.z + 1.2f) / diff);
				} else if (piece.CompareTag ("Rook")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piece.CompareTag ("Knight")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piece.CompareTag ("Bishop")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piece.CompareTag ("Queen")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piece.CompareTag ("King")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piece.CompareTag ("ChessSquare")) {
						return (int)Mathf.RoundToInt ((pos.z + 4.2f) / diff);
				}
		
				return -4;
		
		}

		/*
		 * checks if any of the squares on the path the chess piece takes to its new location, 
		 * has other pieces on it. If there are, then this is an invalid move.
		 * Note that newRow and newCol are already valid in that if there are no pieces on the
		 * path, this would be a valid move.
		 * */ 
		public bool checkPathBetweenMoves (Cell[,] curGrid, int oldRow, int oldCol, int newRow, int newCol, bool player, MoveChessPiece curPiece)
		{
				if (curPiece.CompareTag ("Pawn")) {
						if (Mathf.Abs (oldCol - newCol) == 1 && Mathf.Abs (oldRow - newRow) == 1) { // moving diagonally 
								return (curGrid [newRow, newCol].hasChessPiece);
						} 
						// can't move forward if anything in front of it
						else if (player) { // white
								if (oldRow == 6 && oldCol == newCol) {
										if (newRow == 5) {
												if (!curGrid [newRow, newCol].hasChessPiece)
														return true;
										} else if (newRow == 4) {
												if (!curGrid [newRow, newCol].hasChessPiece && !curGrid [newRow + 1, newCol].hasChessPiece)
														return true;
										}
								} else if (oldRow - newRow == 1 && oldCol - newCol == 0) {
										return !curGrid [newRow, newCol].hasChessPiece;
								}
								
						} else { // black
								if (oldRow == 1 && oldCol == newCol) {
										if (newRow == 2) {
												if (!curGrid [newRow, newCol].hasChessPiece)
														return true;
										} else if (newRow == 3) {
												if (!curGrid [newRow, newCol].hasChessPiece && !curGrid [newRow - 1, newCol].hasChessPiece)
														return true;
										}
								} else if (newRow - oldRow == 1 && oldCol - newCol == 0) {
										return  !curGrid [newRow, newCol].hasChessPiece;
								}
						}
			
				} else if (curPiece.CompareTag ("Rook")) {
						// needs to check if all squares between cur pos and final pos in straight line are free
						
						if (oldRow > newRow) {
								for (int i = newRow + 1; i < oldRow; i++) {
										if (curGrid [i, newCol].hasChessPiece)
												return false;
								}
						} else if (oldRow < newRow) {
								for (int i = oldRow + 1; i < newRow; i++) {
										if (curGrid [i, newCol].hasChessPiece)
												return false;
								}
						} else if (oldCol > newCol) {
								for (int i = newCol + 1; i < oldCol; i++) {
										if (curGrid [newRow, i].hasChessPiece)
												return false;
								}
						} else if (oldCol < newCol) {
								for (int i = oldCol + 1; i < newCol; i++) {
										if (curGrid [newRow, i].hasChessPiece)
												return false;
								}
						}
						return true;
			
				} else if (curPiece.CompareTag ("Knight")) {
						// The knight can jump over pieces, so we always return true here
						return true;
				} else if (curPiece.CompareTag ("Bishop")) {
						// like rook, need to check if all squares between initial and final position have no pieces.
						// bishop
						var diff = Mathf.Abs (oldCol - newCol);
						if (oldRow > newRow && oldCol > newCol) { // moving northwest
				
								for (int i = 1; i < diff; i++) {
										if (curGrid [newRow + i, newCol + i].hasChessPiece)
												return false;
					
								}
						} else if (oldRow > newRow && oldCol < newCol) { // moving northeast
				
								for (int i = 1; i < diff; i++) {
										if (curGrid [oldRow - i, oldCol + i].hasChessPiece)
												return false;
					
								}
						} else if (oldRow < newRow && oldCol > newCol) { // moving southwest
								for (int i = 1; i < diff; i++) {
										if (curGrid [newRow - i, newCol + i].hasChessPiece)
												return false;
					
								}
						} else if (oldRow < newRow && oldCol < newCol) { // moving southeast
								for (int i = 1; i < diff; i++) {
										if (curGrid [oldRow + i, oldCol + i].hasChessPiece)
												return false;
					
								}
						}  
			
						return true;
			
				} else if (curPiece.CompareTag ("Queen")) {
						// test rook or bishop paths
						// first check if queen is moving diagonally or straight
						bool line = false;
						if ((oldCol == newCol && oldRow != newRow) || (oldCol != newCol && oldRow == newRow))
								line = true;
						if (line) {
								// rook 
								if (oldRow > newRow) {
										for (int i = newRow + 1; i < oldRow; i++) {
												if (curGrid [i, newCol].hasChessPiece)
														return false;
										}
								} else if (oldRow < newRow) {
										for (int i = oldRow + 1; i < newRow; i++) {
												if (curGrid [i, newCol].hasChessPiece)
														return false;
										}
								} else if (oldCol > newCol) {
										for (int i = newCol + 1; i < oldCol; i++) {
												if (curGrid [newRow, i].hasChessPiece)
														return false;
										}
								} else if (oldCol < newCol) {
										for (int i = oldCol + 1; i < newCol; i++) {
												if (curGrid [newRow, i].hasChessPiece)
														return false;
										}
								}
								return true;
						} else {
								// bishop
								var diff = Mathf.Abs (oldCol - newCol);
								if (oldRow > newRow && oldCol > newCol) { // moving northwest
										
										for (int i = 1; i < diff; i++) {
												if (curGrid [newRow + i, newCol + i].hasChessPiece)
														return false;
						
										}
								} else if (oldRow > newRow && oldCol < newCol) { // moving northeast
										
										for (int i = 1; i < diff; i++) {
												if (curGrid [oldRow - i, oldCol + i].hasChessPiece)
														return false;
						
										}
								} else if (oldRow < newRow && oldCol > newCol) { // moving southwest
										for (int i = 1; i < diff; i++) {
												if (curGrid [newRow - i, newCol + i].hasChessPiece)
														return false;
						
										}
								} else if (oldRow < newRow && oldCol < newCol) { // moving southeast
										for (int i = 1; i < diff; i++) {
												if (curGrid [oldRow + i, oldCol + i].hasChessPiece)
														return false;
						
										}
								}  
						
								return true;
						}
				} else if (curPiece.CompareTag ("King")) {
						// same movements as queen but limited to 1 square away
						// therefore, path of length 1 so always return true
						return true;
			
				}
				return false;
		}
		
		// check if player is in check
		public  bool isCheck (Cell[,] curGrid, bool player)
		{
				var king = findPiece ("King", player);
				var kingRow = king.row;
				var kingCol = king.col;

				if (checkIfKingInCheck (curGrid, kingRow, kingCol, player)) {
						if (player) 
								whiteCheck = true;
						else 
								blackCheck = true;
						return true;
				}

				return false;
		}


		// checks if player is in checkmate
		public bool isCheckmate (Cell[,] curGrid, bool player)
		{
				// get king pos
				// is king in check? then continue, else return false
				// for each of the empty squares surrounding him, check if 
	
				var king = findPiece ("King", player);
				var kingRow = king.row;
				var kingCol = king.col; 
		
				if (!checkIfKingInCheck (curGrid, kingRow, kingCol, player)) {
						return false;
				}

				if (checkIfKingInCheckmate (curGrid, kingRow, kingCol, player)) {
						
						return true;
				}

				return false;
		}

		MoveChessPiece findPiece (string tag, bool player)
		{
				if (player) {
						foreach (MoveChessPiece piece in whitePieces) {
								if (piece.CompareTag ("King")) {
										return piece;
								}
						}
				} else {
						foreach (MoveChessPiece piece in blackPieces) {
								if (piece.CompareTag ("King")) {
										return piece;
								}
						}
				}
				return null;
		}
		
		// checks if any enemy piece can hit the king in 1 move
		bool checkIfKingInCheck (Cell[,] curGrid, int kingRow, int kingCol, bool player)
		{	
				var pieces = whitePieces;
				if (player) {
						pieces = blackPieces;
				} else {
						pieces = whitePieces;
				}

				// go through each of the pieces and see if they can hit king in 1 move
				foreach (MoveChessPiece piece in pieces) {
						if (piece.isValidMove (!player, kingRow, kingCol) && checkPathBetweenMoves (curGrid, piece.row, piece.col, kingRow, kingCol, !player, piece)) {
								print (piece.name);				
								return true;
						}
				}
				return false;
		}
		
		// checks if all the squares without pieces within distance of 1 from king are reachable by some enemy piece.
		// TO IMPLEMENT : what if enemy pieces within 1 square of king. 
		// also, check if any of the pieces 
		bool checkIfKingInCheckmate (Cell[,] curGrid, int kingRow, int kingCol, bool player)
		{
				var pieces = whitePieces;
				if (player) {
						pieces = blackPieces;
				} else {
						pieces = whitePieces;
				}

				int count = 0;
				int squares = 9;
				for (int i = -1; i < 2; i ++) {
						for (int j = -1; j < 2; j++) {
								
								var r = kingRow + i;
								var c = kingCol + j;
								
								if (r <= 7 && r >= 0 && c <= 7 && c >= 0) { // check if in bounds

										var value = curGrid [r, c].piece;
										if ((!player && ((value > 6 && value < 13) || value == 5)) || (player && ((value > 0 && value < 7) || value == 11))) { // makes sure no allied pieces on square
												
												// go through each of the pieces and see if they can hit king in 1 move
												foreach (MoveChessPiece piece in pieces) {
														// first check if piece is still on board ( when used on children of node)
														if (piece.row >= 0 && piece.col >= 0) { // piece has not been captured  yet
																if (curGrid [piece.row, piece.col].piece == piece.value) {
														
																		if (piece.isValidMove (!player, r, c) && checkPathBetweenMoves (curGrid, piece.row, piece.col, r, c, !player, piece)) {
																				count += 1;
																				break;
																		}
																}
														}
												}
										} else 
												squares -= 1;
								} else
										squares -= 1;

						
						}
				}
				bool check1 = false;
				if (count == squares) {
						check1 = true;
				}
				return check1;
		}


}
