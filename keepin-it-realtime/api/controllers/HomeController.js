/**
 * HomeController
 *
 * @description :: Server-side logic for managing homes
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {


  index: function (req, res) {
    console.log(req.session.user);
    if (req.session.user) {
      User.findOne(req.session.user).then(function(user){
        console.log("user");
        console.log(user);
        User.find().where({ username: { '!': user.username }}).exec(function(err, users){
          if (err) allUsers = [];
          else allUsers = users;
          if (user.friends.length>0){
            User.find().where({id: user.friends}).exec(function(err, friends){
              if (err) friends = []
              return res.view('index', {user: user, allUsers: allUsers, friends: friends});
            });
          }
          else return res.view('index', {user: user, allUsers: allUsers, friends:[]});
        });
      });
    } else {
      return res.view('index', {user: 'rando'});
    }
    //res.view('index');
  }


};

