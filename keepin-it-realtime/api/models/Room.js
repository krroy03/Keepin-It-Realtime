/**
 * Room
 *
 * @module      :: Model
 * @description :: A short summary of how this model works and what it represents.
 *
 */

module.exports = {

  autosubscribe: ['destroy', 'update', 'add:users', 'remove:users'],
  attributes: {

		name: 'string',
		users: {
			collection: 'user',
			via: 'rooms'
		}

  },

  afterPublishRemove: function(id, alias, idRemoved, req) {

  	// Get the room and all its users
  	Room.findOne(id).populate('users').exec(function(err, room) {
  		// If this was the last user, close the room.
  		if (room.users.length === 0) {
  			room.destroy(function(err) {
          // Alert all sockets subscribed to the room that it's been destroyed.
  				Room.publishDestroy(room.id);
  			});
  		}
  	});

  }

};
