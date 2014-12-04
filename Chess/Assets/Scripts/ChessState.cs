using UnityEngine;
using System.Collections;

public class ChessState : ScriptableObject
{
		const int LIGHT = 0;
		const int DARK = 1;
		const int EMPTY = 0;
		const int PAWN_B = 1;
		const int ROOK_B = 2;
		const int KNIGHT_B = 3;
		const int BISHOP_B = 4;
		const int KING_B = 5;
		const int QUEEN_B = 6;
		const int PAWN_W = 7;
		const int ROOK_W = 8;
		const int KNIGHT_W = 9;
		const int BISHOP_W = 10;
		const int KING_W = 11;
		const int QUEEN_W = 12;

		// give each chess piece a value
		int kingVal = 10000;
		int queenVal = 5000;
		int rookVal = 2500;
		int bishopVal = 1500;
		int knightVal = 750;
		int pawnVal = 100;
		public DetectClick.Cell[,] grid ;
		public DetectClick dClick;

		public static ChessState MakeInstance (DetectClick.Cell[,] gridNew, DetectClick dClick)
		{
				ChessState state = ChessState.CreateInstance<ChessState> ();

				state.grid = new DetectClick.Cell[8, 8];

				// make a deep copy of grid now
				for (int i = 0; i < 8; i++) {
						for (int j = 0; j<8; j++) {
								var cur = new DetectClick.Cell ();
								cur.row = i;
								cur.col = j;
								cur.hasChessPiece = gridNew [i, j].hasChessPiece;
								cur.piece = gridNew [i, j].piece;
								state.grid [i, j] = cur;
						}
				}

				state.dClick = dClick;
				return state;

		}
		// Use this for initialization
		void Start ()
		{
				// init grid first
				//dClick = GameObject.FindGameObjectWithTag ("ChessBoard").GetComponent<DetectClick> ();
				
		}
	
		// Update is called once per frame
		void Update ()
		{

		}
		
		public int EvaluateState (bool player)
		{	
				// assign high positive weights to strategies favourable for player
				// assign negative weights to those that are bad for player
				// firstly, we try to maximize the value of pieces that player has and minimize the value of pieces that opponent has
				// so val of all pieces is val of all our pieces - val of opponent's pieces
				int playerVal = 0;
				int aiVal = 0;
				for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {
								switch (this.grid [i, j].piece) {
								case(1):
										aiVal += pawnVal;
										break;
								case(2):
										aiVal += rookVal;
										break;
								case(3):
										aiVal += knightVal;
										break;
								case(4):
										aiVal += bishopVal;
										break;
								case(5):
										aiVal += kingVal;
										break;
								case(6):
										aiVal += queenVal;
										break;
								case(7):
										playerVal += pawnVal;
										break;
								case(8):
										playerVal += rookVal;
										break;
								case(9):
										playerVal += knightVal;
										break;
								case(10):
										playerVal += bishopVal;
										break;
								case(11):
										playerVal += kingVal;
										break;
								case(12):
										playerVal += queenVal;
										break;
								}
						}
				}
				int finalVal = player ? (playerVal - aiVal*2) : (aiVal - playerVal*2);
				// now check if either player is in check or checkmate
				if (dClick.isCheckmate (grid, player)) {
						// we don't want this , give it low score
						finalVal -= 1000000;
				} else if (dClick.isCheckmate (grid, !player)) {
						// perfect!
						finalVal += 1000000;
				} else if (dClick.isCheck (grid, player)) {
						finalVal -= 100000;
				} else if (dClick.isCheck (grid, !player)) {
						finalVal += 100000;
				}

				// also, if we can take one of the opponents pieces, assign a high value, equal to twice the value of piece
				MoveChessPiece[] player_pieces = null;
				MoveChessPiece[] ai_pieces = null;
				if (player) {
						player_pieces = dClick.whitePieces;
						ai_pieces = dClick.blackPieces;
				} else {
						player_pieces = dClick.blackPieces;
						ai_pieces = dClick.whitePieces;
				}

				


				return Random.Range (0, 100) + finalVal;

		}


}
