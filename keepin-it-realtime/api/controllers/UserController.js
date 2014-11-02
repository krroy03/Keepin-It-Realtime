/**
 * UserController
 *
 * @description :: Server-side logic for managing users
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {
	


  /**
   * `UserController.create()`
   */
  create: function (req, res) {

    var userObj = {
      username: "Random"
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
        return res.redirect('/');
      }


      // Let other subscribed sockets know that the user was created.
      User.publishCreate(user);

      // After successfully creating the user
      // redirect to the show action
      // From ep1-6: //res.json(user); 

      res.redirect('/user/show/' + user.id);
    });
  },


  /**
   * `UserController.show()`
   */
  show: function(req, res, next) {
    User.findOne(req.param('id'), function foundUser(err, user) {
      if (err) return next(err);
      if (!user) return next();
      res.view({
        user: user
      });
    });
  },


  /**
   * `UserController.subscribe()`
   */
  subscribe: function (req, res) {
    return res.json({
      todo: 'subscribe() is not implemented yet!'
    });
  }
};

