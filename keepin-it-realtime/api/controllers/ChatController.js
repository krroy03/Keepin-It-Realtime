  module.exports = {

	// Post a message in a public chat room
	public: function(req, res) {
		// Get the ID of the currently connected socket
		var socketId = sails.sockets.id(req.socket);
		// Use that ID to look up the user in the session
		// We need to do this because we can have more than one user
		// per session, since we're creating one user per socket
		User.findOne(req.session.user).exec(function(err, user) {
			// Publish a message to the room's "room".  Every user in the room will have their socket
			// subscribed to it, so they'll all get the message.  The user who created the room gets
			// their socket subscribed to it in RoomController.create; everyone who joins later gets
			// subscribed automatically in the "add" blueprint for the Room model.
			Room.message(req.param('room'), {room:{id:req.param('room')}, from: user, msg: req.param('msg')}, req.socket);

		});

	}

};