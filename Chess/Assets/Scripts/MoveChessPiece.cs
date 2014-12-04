using UnityEngine;
using System.Collections;

public class MoveChessPiece : MonoBehaviour
{

		public Transform piecePos;
		private int startingRow;
		private int startingCol;
		public int row; // row on which piece is on
		public int col; // col on which piece is on
		public int savedRow;
		public int savedCol;
		private float diff = 1.2f;
		public string pieceName;
		public int value;
		public bool player; // true means white piece, false means black piece 
		// Use this for initialization
		
		public DetectClick dClick;

		void Start ()
		{
				setUpPiece (false);	
		}
		
		public void setUpPiece (bool reset)
		{		
				if (reset) {
						row = startingRow;
						col = startingCol;
						MovePiece ();
				} else {
						piecePos = this.transform;
						row = getRowInfo ();
						col = getColInfo ();
						startingRow = row;
						startingCol = col;
				}
				pieceName = this.tag;
				if (player) {
						if (pieceName.Equals ("Pawn"))
								value = 7;
						if (pieceName.Equals ("Rook"))
								value = 8;
						if (pieceName.Equals ("Knight"))
								value = 9;
						if (pieceName.Equals ("Bishop"))
								value = 10;
						if (pieceName.Equals ("King"))
								value = 11;
						if (pieceName.Equals ("Queen"))
								value = 12;
			
				} else {
						if (pieceName.Equals ("Pawn"))
								value = 1;
						if (pieceName.Equals ("Rook"))
								value = 2;
						if (pieceName.Equals ("Knight"))
								value = 3;
						if (pieceName.Equals ("Bishop"))
								value = 4;
						if (pieceName.Equals ("King"))
								value = 5;
						if (pieceName.Equals ("Queen"))
								value = 6;
				}
		}
		// Update is called once per frame
		void Update ()
		{
		
		}
	
		/*
	 * this method only gets called when piecePos and squarePos have valid values
	 * we need to move the piecePos into squarePos. 
	 * */
		public void MovePiece ()
		{
				// use new row and col to get new position of piece
				
				Vector3 test = Vector3.zero;
				if (this.CompareTag ("Pawn")) {
						test = new Vector3 ((row - 7) * diff, piecePos.position.y, (col - 1) * diff);
				} else if (this.CompareTag ("Rook")) {
						test = new Vector3 ((row - 7) * diff, piecePos.position.y, col * diff);
				} else if (this.CompareTag ("Knight")) {
						test = new Vector3 ((row - 6) * diff, piecePos.position.y, col * diff);
				} else if (this.CompareTag ("Bishop")) {
						test = new Vector3 ((row - 5) * diff, piecePos.position.y, col * diff);
				} else if (this.CompareTag ("Queen")) {
						test = new Vector3 ((row - 4) * diff, piecePos.position.y, col * diff);
				} else if (this.CompareTag ("King")) {
						test = new Vector3 ((row - 3) * diff, piecePos.position.y, col * diff);
				}
				piecePos.position = test;
				print ("moved piece");
				//print (test);
		}

		public void RemovePiece ()
		{
				var add = (float)(Random.Range (1, 10) / 4.0f);
				piecePos.position = new Vector3 (0f, 0.2f, 30.0f + add);
		savedRow = row;
		savedCol = col;
				row = -2;
				col = -2;
		}

	public void UndoRemovePiece() {
		row = savedRow;
		col = savedCol;
		MovePiece ();

		}
		


		public bool isValidMove (bool player, int newRow, int newCol)
		{		
				
			if (player && dClick.whiteCheck && !piecePos.CompareTag ("King")) {// if white is checked, can only move king 
				// test if move makes board not in check anymore for player, else return false
							return false;
			}
				if (!player && dClick.blackCheck && !piecePos.CompareTag ("King")) // if black is checked, can only move king
						return false;

				if (piecePos.CompareTag ("Pawn")) {
						if (player) { // white
								if (row == 6 && (newRow == 5 || newRow == 4)) {
										return true;
								}
								if (row - newRow == 1)
										return true;
								if (row - newRow == 1 && (Mathf.Abs (col - newCol) == 1)) 
										return true;
						} else { // black
								if (row == 1 && (newRow == 2 || newRow == 3)) {
										return true;
								}
								if (newRow - row == 1)
										return true;
								if (newRow - row == 1 && (Mathf.Abs (col - newCol) == 1)) 
										return true;
						}
					
				} else if (piecePos.CompareTag ("Rook")) {
						if ((col == newCol && row != newRow) || (col != newCol && row == newRow))
								return true;

				} else if (piecePos.CompareTag ("Knight")) {
						// 8 cases 
						if (Mathf.Abs (newRow - row) == 2) { // knight going 2 up or down 
								if (Mathf.Abs (newCol - col) == 1) // then 1 left or right
										return true;
						}
						if (Mathf.Abs (newCol - col) == 2) { // knight going 2 left or right 
								if (Mathf.Abs (newRow - row) == 1) // then 1 up or down
										return true;
						}
				} else if (piecePos.CompareTag ("Bishop")) {
						// can only go diagonal, so opposite of rooks
						if (col != newCol && row != newRow) {
								if (Mathf.Abs (col - newCol) == Mathf.Abs (row - newRow))
										return true;
						}
		
				} else if (piecePos.CompareTag ("Queen")) {
						// test rook or bishop movements
						// rook 
						if ((col == newCol && row != newRow) || (col != newCol && row == newRow)) 
								return true;
						// bishop
						if (col != newCol && row != newRow) {
								if (Mathf.Abs (col - newCol) == Mathf.Abs (row - newRow))
										return true;
						}
				} else if (piecePos.CompareTag ("King")) {
						// same movements as queen but limited to 1 square away
						if (Mathf.Abs (row - newRow) <= 1 && Mathf.Abs (col - newCol) <= 1)
								return true;

				}
				return false;
		}

		int getRowInfo ()
		{
				var pos = piecePos.position;
				if (piecePos.CompareTag ("Pawn")) {
						return (int)Mathf.RoundToInt ((pos.x + 8.4f) / diff);
				} else if (piecePos.CompareTag ("Rook")) {
						return (int)Mathf.RoundToInt ((pos.x + 8.4f) / diff);
				} else if (piecePos.CompareTag ("Knight")) {
						return (int)Mathf.RoundToInt ((pos.x + 7.2f) / diff);
				} else if (piecePos.CompareTag ("Bishop")) {
						return (int)Mathf.RoundToInt ((pos.x + 6.0f) / diff);
				} else if (piecePos.CompareTag ("Queen")) {
						return (int)Mathf.RoundToInt ((pos.x + 4.8f) / diff);
				} else if (piecePos.CompareTag ("King")) {
						return (int)Mathf.RoundToInt ((pos.x + 3.6f) / diff);
				}
				return -3;
		}
		
		int getColInfo ()
		{	

				var pos = piecePos.position;
				if (piecePos.CompareTag ("Pawn")) {
						return (int)Mathf.RoundToInt ((pos.z + 1.2f) / diff);
				} else if (piecePos.CompareTag ("Rook")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piecePos.CompareTag ("Knight")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piecePos.CompareTag ("Bishop")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piecePos.CompareTag ("Queen")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				} else if (piecePos.CompareTag ("King")) {
						return (int)Mathf.RoundToInt (pos.z / diff);
				}
				return -3;
			
		}
}
