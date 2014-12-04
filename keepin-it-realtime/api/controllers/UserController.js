/**
 * UserController
 *
 * @description :: Server-side logic for managing users
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */
var passport = require('passport');

module.exports = {



  /**
   * `UserController.create()`
   */

  create: function (req, res) {
    var session_user = req.session.user;
    var is_guest = req.param('quest');
    var username = req.param('username')
      , password = req.param('password');

    if (!username) {
      console.log(new Error('No password was entered.'));
      req.session.flash = {
        err: new Error('No username was entered.')
      }

      // If error redirect back to sign-up page
      return res.view('index', {user: 'rando', errors: ["Please include a username"] });
    }

    if (!password) {
      console.log(new Error('No password was entered.'));
      req.session.flash = {
        err: new Error('No password was entered.')
      }

      // If error redirect back to sign-up page
      return res.view('index', {user: 'rando', errors: ["Please include a password"] });
    }

    // If session has a user, dont create another
    if (session_user) {
      console.log("User already created", session_user);
      res.redirect('/');
    }

    // Only create user if there is no session user
    else {
      if (is_guest) {
        var userObj = {
          username: username,
          password: password
        }
      }
      else {
        var userObj = {
          username: username,
          password: password
        }
      }


      User.create(userObj, function userCreated(err, user) {
        // // If there's an error
        // if (err) return next(err);

        if (err) {
          console.log(err);
          req.session.flash = {
            err: err
          }

          // If error redirect back to sign-up page
          return res.view('index', {user: 'rando', errors: ["Username already in use"] });
        }
        console.log("User created");
        req.session.user = user.id;
        var t = new Date();
        t.setDate(t.getDate() + 1 );
        // t.setSeconds(t.getSeconds() + 10);
        req.session.expires = t;

        // Let other subscribed sockets know that the user was created.
        User.publishCreate(user);

        passport.authenticate('local', function(err, user, info) {
          if ((err) || (!user)) {
            return res.send({
            message: 'login failed'
            });
            res.send(err);
          }
          req.logIn(user, function(err) {
            if (err) return res.redirect('/');
            req.session.user = req.session.passport.user;
            return res.redirect('/');
          });
        })(req, res);

        // After successfully creating the user
        // redirect to the show action
        // From ep1-6: //res.json(user);

      });
    }

  },


  // Get current user session
  current_user: function(req, res) {
    var session_user = req.session.passport.user;
    return res.json({
      user: session_user
    });
  },

  // Get current user session
  current_user_object: function(req, res) {
    User.findOne(req.session.passport.user, function foundUser(err, user) {
      if (err || !user) user = {};

      return res.json({
        user: user
      });
    });

  },

  // Get scores
  get_scores: function(req, res) {
    var session_user = req.session.passport.user;
    if (session_user) {
      Score.find().where({user_id: session_user}).exec(function (err, scores) {
        console.log(scores);
        if (err || !scores) {
          console.log(err);
          res.redirect('/');
        }
        else {
          return res.json({
            scores: scores
          });
        }
      });
    }
    else {
      res.redirect('/');
    }

  },


  /**
   * `UserController.show()`
   */
  show: function(req, res, next) {
    User.findOne(req.param('id'), function foundUser(err, user) {
      if (err) return next(err);
      if (!user) return next();
      User.find().where({id: user.friends}).exec(function(err, friends){
        if (err) friends = []

        Score.find().where({user: user.id}).exec(function(err, scores) {
          console.log(scores);
          if (err) scores = []

          return res.view({user: user, scores: scores, friends: friends});

        });
      });
    });
  },

  messages: function(req, res, next) {
    if (req.user && req.user.id){
      User.findOne(req.user.id, function foundUser(err, user) {
        if (err) return next(err);
        if (!user) return next();
        res.view({
          user: user
        });
      });
    }
  },

  /**
   * `UserController.destroy()`
   */
  destroy: function(req, res, next) {
    console.log("find user with id: "+req.param('id')+"...");
    User.findOne(req.param('id'), function foundUser(err, user) {
      console.log(" GOT ERROR "+ err);
      if (err) return next(err);
      console.log(" GOT USER "+ user);
      if (!user) return next('User doesn\'t exist.');

      User.destroy(req.param('id'), function userDestroyed(err) {
        if (err) return next(err);



        // Inform other sockets (e.g. connected sockets that are subscribed) that this user is now logged in
        User.publishUpdate(user.id, {
          name: user.name,
          action: ' has been destroyed.'
        });

        // Let other sockets know that the user instance was destroyed.
        User.publishDestroy(user.id);

      });

      res.redirect('/user');

    });
  },

  addFriend: function(req, res){
    console.log('addFriend')
    errors = []
    if (('id' in req.params) && (typeof(parseInt(req.params['id']))==='number')){
      friendId = parseInt(req.params['id'])
      User.findOne()
          .where({'id': req.user.id})
          .then(function(user){
            if (user.friends.indexOf(friendId) === -1){
              user.friends.push(friendId)
              user.save(function(err){
                if (err) console.log(err)
              });
            }
          });

    }
    else{
      console.log('no id found in request')
    }

    res.send({success: true});

  },

  viewMessages: function(req, res){
    if (req.session.user){
      User.findOne(req.session.user).then(function(user){
        if (!user) return res.redirect('/');

        User.find().where({id: user.friends}).exec(function(err, friends){
          if (err) friends = []
          if (('id' in req.params) && req.params['id']){
            friendId = req.params['id']
          }
          else{
            friendId = -1
          }
          res.view('message/messages', {user: user, friends:friends, friendId: friendId})
        });
      });
    }
    else{
      res.redirect('/');
    }
  },

  sendMessage: function(req, res){
    errors = []
    if (('id' in req.params) && (typeof(parseInt(req.params['id']))==='number')){
      toid = parseInt(req.params['id'])
      if (toid !== -1){
        User.findOne()
            .where({'id': toid})
            .then(function(user){
              if (!user) return res.redirect('/');
              var message = {}
              message.text = req.body.message;
              message.to = user.username;
              message.from = req.user.username;
              message.date = new Date();

              if (!user.messages[req.user.id]){  // May cause trouble???
                user.messages[req.user.id] = []
              }
              user.messages[req.user.id].push(message)
              user.save(function(err){
                if (err) console.log(err)
                else {
                  User.findOne(req.session.user)
                    .then(function(sender){
                      if(!sender) return res.redirect('message/'+user.id)
                      if (!sender.messages[user.id]){
                        sender.messages[user.id] = []
                      }
                      sender.messages[user.id].push(message)
                      sender.save(function(err){
                        if (err) console.log(err)
                      });
                    });
                }
              });
            });
      }
      else{
        console.log('user attempting to send message to user.id=-1')
      }
    }
    else{
      console.log('no id found in request')
    }
    res.send({'success':true})
  },

  /**
   * `UserController.subscribe()`
   */
  subscribe: function (req, res) {
    return res.json({
      todo: 'subscribe() is not implemented yet!'
    });
  },

  /**
  * `UserController.editProfile()`
  */
  editProfile: function(req, res) {
    User.findOne(req.session.user).then(function(user) {
      return res.view('user/edit', {
        user: user,
      });
    });
  },

  updateProfile: function(req, res) {
    if (req.method == 'POST') {
      var displayname = req.param('inputDisplayname');
      var aboutme = req.param('inputAboutme');
      var propic = req.file('propic');
      User.findOne(req.session.user).then(function(user) {
        if (displayname) {
          user.displayname = displayname;
          user.save(function(err){if (err) console.log(err)});
        }
        if (aboutme) {
          user.aboutme = aboutme;
          user.save(function(err){if (err) console.log(err)});
        }

        if (propic) {
          propic.upload({ dirname: '../../assets/images/avatar'}, function onUploadComplete (err, files) {
            if (err) return res.serverError(err);
            var path_res = files[0]['fd'].split('/');
            var propic_path = '/' + path_res[path_res.length - 3] + '/' + path_res[path_res.length - 2] + '/' + path_res[path_res.length - 1];
            user.propic = propic_path;
            user.save(function(err){if (err) console.log(err)});
            res.json({status:200,file:files});
          });
        }

      });
      res.view('user/confirm');
    } else {
      res.redirect('/');
    }
  },
};

