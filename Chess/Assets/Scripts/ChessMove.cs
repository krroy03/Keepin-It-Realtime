using UnityEngine;
using System.Collections;

public class ChessMove : ScriptableObject  {

	public ChessState before;
	public ChessState after; 

	public int fromRow;
	public int fromCol;

	public int curRow;
	public int curCol;

	public MoveChessPiece attacked;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static ChessMove MakeInstance(ChessState before, int oldRow, int oldCol, int newRow, int newCol) {
		ChessMove move = ChessMove.CreateInstance<ChessMove>();
		move.before = ChessState.MakeInstance (before.grid, before.dClick);
		move.after = ChessState.MakeInstance (before.grid, before.dClick);
		move.fromRow = oldRow;
		move.fromCol = oldCol;
		move.curRow = newRow;
		move.curCol = newCol;
		return move;
		}
		
		

	public void GetAfterState(MoveChessPiece piece) {
		// assume that after state is still in initalizied state, meaning that it has before state
		after.grid [fromRow, fromCol].hasChessPiece = false;
		after.grid [fromRow, fromCol].piece = 0;
		after.grid [curRow, curCol].hasChessPiece = true;
		after.grid [curRow, curCol].piece = piece.value;
	}
}
