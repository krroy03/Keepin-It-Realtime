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
        res.view({username: req.session.user.username});
    } else {
      res.view({username: 'rando'});
    }
    res.view('index');
  }


};

