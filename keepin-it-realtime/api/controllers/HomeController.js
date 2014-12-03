/**
 * HomeController
 *
 * @description :: Server-side logic for managing homes
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {


  index: function (req, res) {
    errors=[]
    if (req.session.user) {
      allUsers=[]
      allFriends=[]
      allScores=[]
      User.findOne(req.session.user).then(function(user){
        if(!user){
          console.log(user)
          res.session.user = undefined
          res.redirect('/')
        }
        else{
          User.find().where({ username: { '!': user.username }}).exec(function(err, users){
            if (err) errors += [err]
            else allUsers = users;
            Score.find().where({user: user.id}).exec(function(err, scores) {
              if (err) errors += [err]
              else allScores = scores

              if (user.friends.length>0){
                User.find().where({id: user.friends}).exec(function(err, friends){
                  if (err) errors += [err]
                  else allFriends = friends
                  return res.view("index", {user: user, allUsers: allUsers, scores: allScores, friends: allFriends, errors: errors});
                });
              }
              else return res.view("index", {user: user, allUsers: allUsers, scores: allScores, friends: allFriends, errors: errors});
            });
          });
        }
      });
    } else {
      return res.view('index', {user: 'rando', errors: errors});
    }
    //res.view('index');
  }


};
