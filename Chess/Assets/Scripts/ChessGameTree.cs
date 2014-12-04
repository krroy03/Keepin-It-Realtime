using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessGameTree : MonoBehaviour
{
		public int maxSearchDepth = 3;
		public DetectClick dClick; 

		public class Node
		{
				// a node in the tree, contains a chessMove 
				public ChessMove move; // move.after is the gamestate of this mode. move.before is the node from which we came here
				public ChessState state;
				public int weight; // weight of 0 represents root 

				public List<Node> children; // list of all nodes which are 1 move away
				public ChessMove bestMove; // the best move from this node
				public Node best; // the node moved to by bestMove
				public bool playerTurn; // true if white, false if black (black always ai), this is the current player turn at this state
				public Node (ChessMove move, int weight, bool playerTurn)
				{ // initializes a node without any neighbours
						this.move = move;
						this.weight = weight;
						this.state = move.after;
						this.playerTurn = playerTurn;
				}
		}

		public Node root;
		// Use this for initialization
		void Start ()
		{
				
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		public Node makeCurNode (int newRow, int newCol, bool player)
		{
				var state = ChessState.MakeInstance (dClick.grid, dClick);
				var move = ChessMove.MakeInstance (state, newRow, newCol, newRow, newCol);
				var weight = 0;
				if (newRow >= 0 || newCol >= 0)
						weight = state.EvaluateState (player);

				return new Node (move, weight, player);
		
		}

	
		/* 
		 * main function to call on a node for ai, to find next best move
		 * */
		public Node RunAlphaBeta (Node node, int depth, int alpha, int beta, bool maximizer)
		{
				print ("1st iteration of findchildren");

				var children = FindChildren (node);

				node.children = children;
				var bestVal = int.MinValue;
				var bestNode = node;
				float prob = 0f;
				bool foundSingularBest = false;
				List<Node> bestNodes = new List<Node> ();
				foreach (Node child in node.children) {
						var value = alphabeta (child, depth, alpha, beta, !maximizer);
						print ("iteration of findchildren");
						if (value > bestVal) { 
								bestVal = value;
								bestNode = child;
								foundSingularBest = true;
								bestNodes.Clear (); // clear the current list every time we find a new bestVal
								bestNodes.Add (child);
						} else if (value == bestVal) { // get all nodes that have same best score
								bestNodes.Add (child);
				
						} 
				}
				
				if (bestNodes.Count > 1) {
						int randIndex = Random.Range (0, bestNodes.Count - 1); // uniformly pick a new best move at random
						node.best = bestNodes [randIndex];
						node.bestMove = node.best.move;
						return node.best;
				} else {
						// now we have the best node to move to
						node.best = bestNode;
						node.bestMove = bestNode.move;
						return bestNode;
				}
				
		}
	
		/*
	 * runs the alpha beta pruning algorithm up to maxdepth, and either tries to minimize or maximize depending on player 
	 * What this returns is the best value for a particular node. 
	 * When we are at a node, we run this algorithm on each of its children, and then pick the node with the best score
	 * to be the next move.
	 * */
		int alphabeta (Node node, int depth, int alpha, int beta, bool maximizer)
		{	
				//print (depth);
				if (depth == 0 || dClick.isCheckmate (node.state.grid, maximizer)) {
						return node.weight;
				}

				var children = FindChildren (node);
				if (maximizer) {
			
						foreach (Node child in children) {
								alpha = Mathf.Max (alpha, alphabeta (child, depth - 1, alpha, beta, !maximizer));
								if (beta <= alpha) 
										break; // beta cut-off
				
						}
						return alpha;
				} else {
						foreach (Node child in children) {
								beta = Mathf.Min (beta, alphabeta (child, depth - 1, alpha, beta, !maximizer));
								if (beta <= alpha) 
										break; // beta cut-off
				
						}
						return beta;
				}
		}
	
		/*
	 * returns all the possible legal moves that can be made from a certain state
	 * 1) finds current player
	 * 2) goes through all pieces on the board belonging to that player
	 * 3) for each piece, make a new node for each legal move of that piece
	 * 4) return the list of all these nodes
	 * */
		public	List<Node> FindChildren (Node node)
		{
				var curBoard = node.state.grid;
				var children = new List<Node> ();
				var allMoves = new List<ChessMove> ();
				// loop through all squares
				for (int row = 0; row < 8; row++) {
						for (int col = 0; col < 8; col ++) {
								if (!curBoard [row, col].hasChessPiece) 
										continue;
								if (node.playerTurn && curBoard [row, col].piece > 6) { // only add legal moves from white pieces
										allMoves = FindAllLegalMoves (node.state, row, col, node.playerTurn);
										foreach (ChessMove move in allMoves) {
												// make a new node with each of them and add to list of neighbours
												children.Add (new Node (move, move.after.EvaluateState (node.playerTurn), !node.playerTurn));
										}	
								} else if (!node.playerTurn && curBoard [row, col].piece < 7) { // only add legal moves from black pieces

										allMoves = FindAllLegalMoves (node.state, row, col, node.playerTurn);
										foreach (ChessMove move in allMoves) {
												// make a new node with each of them and add to list of neighbours
												children.Add (new Node (move, move.after.EvaluateState (node.playerTurn), !node.playerTurn));
										}	
								}
						}
				}	
		
		
		
				return children;

		}

		bool isAttackingAlly (ChessState state, MoveChessPiece curPiece, int newRow, int newCol)
		{
				var curBoard = state.grid;
				if (!(0 <= newRow && newRow <= 7 && 0 <= newCol && newCol <= 7)) {
						return false;
				}
				var value = curBoard [newRow, newCol].piece;
				if (value > 0 && value < 7 && curPiece.value < 7) { // black attacking black
						return true;
				} else if (value > 6 && curPiece.value > 6) { // white attacking white
						return true;
				}
				return false;
		}
		/*
	 * returns all the legal moves that can be made from a certain game state by player
	 * */
		List<ChessMove> FindAllLegalMoves (ChessState state, int row, int col, bool player)
		{		
				
				var curBoard = state.grid;
				MoveChessPiece curPiece = null;
				List<ChessMove> allLegalMoves = new List<ChessMove> ();
				// find cur piece at row and col of chess board, and find all legal moves of that piece
				var pieces = dClick.whitePieces;
				if (!player) 
						pieces = dClick.blackPieces;
				
				foreach (MoveChessPiece piece in pieces) {
						if (piece.row == row && piece.col == col) {
								curPiece = piece;

						}
				}
				
				int newRow = 0;
				int newCol = 0;
				ChessMove newMove = null;
				// we have the cur piece to move
				if (curPiece.CompareTag ("Pawn")) {
						var sign = 0;
						if (player) 
								sign = -1;
						else
								sign = 1;
					
						// pawn moves straight ahead or diagonally 1 to hit an enemy
						newRow = curPiece.row + sign * 1;
						for (int i = -1; i < 2; i ++) {
								newCol = curPiece.col + i;
								newMove = generateNewMove (state, newRow, newCol, curPiece, player);
								if (newMove != null) 
										allLegalMoves.Add (newMove);
						}

						// pawn moves up 2 spots at start
						newRow = curPiece.row + sign * 2;
						newCol = curPiece.col;
						newMove = generateNewMove (state, newRow, newCol, curPiece, player);
						if (newMove != null) 
								allLegalMoves.Add (newMove);
		
						//print ("found pawn's move");	
				}
				if (curPiece.CompareTag ("Rook") || curPiece.CompareTag ("Queen")) {
						// look at each of the 4 directions, and check all squares within bounds in each direction
						newRow = curPiece.row;
						newCol = 0;
						for (int i = 0; i < 8; i ++) {
								if (i != curPiece.col) {
										newCol = i;
										newMove = generateNewMove (state, newRow, newCol, curPiece, player);
										if (newMove != null) 
												allLegalMoves.Add (newMove);
								}
						}
								
						newCol = curPiece.col;
						newRow = 0;
						for (int i = 0; i < 8; i ++) {
								if (i != curPiece.row) {
										newRow = i;
										newMove = generateNewMove (state, newRow, newCol, curPiece, player);
										if (newMove != null) 
												allLegalMoves.Add (newMove);
								}
						}
						//print ("found rook's move");
				}
				if (curPiece.CompareTag ("Knight")) {
						// check each of the 8 possible spots a knight can go to, as long as within bounds
						// first check moves to top and bottom 4 spots
						newRow = curPiece.row;
						newCol = curPiece.col;
						for (int i = -2; i < 3; i+=2) {
								for (int j = -1; j < 2; j+=2) {
										newRow = curPiece.row + i;
										newCol = curPiece.col + j;
										newMove = generateNewMove (state, newRow, newCol, curPiece, player);
										if (newMove != null) 
												allLegalMoves.Add (newMove);
								}
						}
							

						// now check moves to left and right 4 spots
						newRow = curPiece.row;
						newCol = curPiece.col;
						for (int i = -2; i < 3; i+=2) {
								for (int j = -1; j < 2; j+=2) {
										newRow = curPiece.row + j;
										newCol = curPiece.col + i;
										newMove = generateNewMove (state, newRow, newCol, curPiece, player);
										if (newMove != null) 
												allLegalMoves.Add (newMove);
								}
						}
						//print ("found knights's move");
				}
				if (curPiece.CompareTag ("Bishop") || curPiece.CompareTag ("Queen")) {
						// for all moves, check each of the 4 diagonal directions
						newRow = 0;
						newCol = 0;
						// southeast and southwest first
						for (int i = curPiece.row; i < 8; i ++) {
								newRow = i;
								newCol = curPiece.col + (newRow - curPiece.row);
								newMove = generateNewMove (state, newRow, newCol, curPiece, player);
								if (newMove != null) 
										allLegalMoves.Add (newMove);
								newCol = curPiece.col - (newRow - curPiece.row);
								newMove = generateNewMove (state, newRow, newCol, curPiece, player);
								if (newMove != null) 
										allLegalMoves.Add (newMove);
						}

						// now northeast , northwest 
						for (int i = curPiece.row; i >-1; i --) {
								newRow = i;
								newCol = curPiece.col + (newRow - curPiece.row);
								newMove = generateNewMove (state, newRow, newCol, curPiece, player);
								if (newMove != null) 
										allLegalMoves.Add (newMove);
								newCol = curPiece.col - (newRow - curPiece.row);
								newMove = generateNewMove (state, newRow, newCol, curPiece, player);
								if (newMove != null) 
										allLegalMoves.Add (newMove);
						}
						//print ("found bishop's move");
				}
				if (curPiece.CompareTag ("King")) {
						// this is simply each of the 8 squares around it that are in bounds
						for (int i = -1; i <2; i++) {
								for (int j = -1; j<2; j++) {
										if (i != 0 && j != 0) {
												newRow = curPiece.row + i;
												newCol = curPiece.col + j;
												newMove = generateNewMove (state, newRow, newCol, curPiece, player);
												if (newMove != null) 
														allLegalMoves.Add (newMove);
										}
								}
						}
						//print ("found king's move");
				}
				return allLegalMoves;
			
		}

		ChessMove generateNewMove (ChessState state, int newRow, int newCol, MoveChessPiece curPiece, bool player)
		{
				bool legal1 = false;
				bool legal2 = false;
				
				var newState = state;
				legal1 = (0 <= newRow && newRow <= 7 && 0 <= newCol && newCol <= 7) && curPiece.isValidMove (player, newRow, newCol);
				
				if (legal1) {
						legal2 = !isAttackingAlly (newState, curPiece, newRow, newCol) && dClick.checkPathBetweenMoves (newState.grid, curPiece.row, curPiece.col, newRow, newCol, player, curPiece); 
						if (legal2) {
								var newMove = ChessMove.MakeInstance (newState, curPiece.row, curPiece.col, newRow, newCol);
								newMove.GetAfterState (curPiece);

								return newMove;
						}
				}

				return null;
		
		} 

}
