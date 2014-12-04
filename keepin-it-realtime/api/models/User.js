var bcrypt = require('bcrypt');

module.exports = {
  //autosubscribe: ['destroy', 'update'],
  attributes: {
    username: {
      type: 'string',
      required: true,
      unique: true
    },
    password: {
      type: 'string',
      required: true
    },
    friends:{
      type: 'array',
      defaultsTo: []
    },
    pendingFriends:{
      type: 'array',
      defaultsTo: []
    },
    messages:{
      type: 'json',
      defaultsTo: {}
    },

    aboutme: {
      type: 'string',
      defaultsTo: 'This is my profile.'
    },

    displayname: {
      type: 'string',
      defaultsTo: ''
    },
    propic: {
      type: 'string',
      required: false,
      defaultsTo: '/images/avatar/user_avatar.jpg'
    },
    rooms: {
      collection: 'room',
      via: 'users',
      dominant: true
    },
    // friends: {
    //   collection: 'user',
    //   via: 'friends',
    //   dominant: true
    // },
    toJSON: function() {
      var obj = this.toObject();
      delete obj.password;
      return obj;
    }
  },

  beforeCreate: function(user, cb) {
    bcrypt.genSalt(10, function(err, salt) {
      bcrypt.hash(user.password, salt, function(err, hash) {
        if (err) {
          console.log(err);
          cb(err);
        }else{
          user.password = hash;
          cb(null, user);User
        }
      });
    });
  }

  // tell chats about user update
  /*afterPublishUpdate: function (id, changes, req, options) {

    // Get the full user model, including what rooms they're subscribed to
    User.findOne(id).populate('rooms').exec(function(err, user) {
      // Publish a message to each room they're in.  Any socket that is
      // subscribed to the room will get the message. Saying it's "from" id:0
      // will indicate to the front-end code that this is a systen message
      // (as opposed to a message from a user)
      sails.util.each(user.rooms, function(room) {
        var previousName = options.previous.name == 'unknown' ? 'User #'+id : options.previous.name;
        Room.message(room.id, {room:{id:room.id}, from: {id:0}, msg: previousName+" changed their name to "+changes.name}, req);
      });

    });

  }*/

};
