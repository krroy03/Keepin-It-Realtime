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
      User.findOne(req.session.user, function foundUser(err, user) {
      if (err) {
        console.log("no user found");
      }
      res.view('index', {username: user.username});
    });
    } else {
      res.view('index', {username: 'rando'});
    }
    //res.view('index');
  }


};

