/**
 * ScoreController
 *
 * @description :: Server-side logic for managing scores
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {

    create_or_update: function (req, res) {
    var user_id = req.param('UserID')
      , score = req.param('Score');

    if (!user_id) {
      console.log(new Error('No username was entered.'));
      req.session.flash = {
        err: new Error('No username was entered.')
      }

      // If error redirect back to sign-up page
      return res.redirect('/');
    }

    if (!score) {
      console.log(new Error('No score was entered.'));
      req.session.flash = {
        err: new Error('No score was entered.')
      }

      // If error redirect back to sign-up page
      return res.redirect('/');
    }

    if (user_id && score) {
      var scoreObj = {
        user_id: user_id,
        score: score
      }
      Score.findOne().where({user_id: user_id}).exec(function (err, curr_score) {
        if (curr_score) {
          curr_score.score = score;
          curr_score.save(function(error) {
            if(error) {
                // do something with the error.
            } else {
                // value saved!
              res.redirect('/');
            }
          });
        }
        else {
          Score.create(scoreObj, function scoreCreated(err, score) {
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
            console.log("Score created");

            // After successfully creating the user
            // redirect to the show action
            console.log(score);
            res.redirect('/');

          });
        }
      });
    }

  },

  showAll: function(req, res) {
    var rn = Math.floor(Math.random() * 999999) + 1
    var username = "rando" + String(rn);
    Score.find(function foundScores(err, scores) {
      if (err) {
        console.log(err);
        res.redirect('/');
      }
      else {
        /*User.findOne(req.session.user).then(function(err, user) {
          console.log("username");
          username = user.username;
          return res.view({scores: scores, username: username});
        });*/
        //console.log(username);
        res.view({scores: scores, username: username});
      }
    });

    /*if (req.session.user) {
      User.find(function foundUser(err, user) {
        if (err) {
          console.log("no user found");
        } else {
          res.view({username: user.username});
        }
      });
    } else {
      res.view({username: username});
    }*/
  }

	// update: function(req, res) {
 //    var user_id = req.param('UserID');
 //      , score = req.param('Score');

 //    if (user_id && score) {
 //      var user = User.findOne(user_id).done(function(error, user) {
 //        if(error) {
 //            // do something with the error.
 //        }

 //        if(req.param) {
 //            // validate whether the email address is valid?

 //            // Then save it to the object.

 //        }
 //        // Repeat for each eligible attribute, etc.

 //        user.save(function(error) {
 //          if(error) {
 //              // do something with the error.
 //          } else {
 //              // value saved!
 //              req.send(user);
 //          }
 //        });
 //      });
 //    }
 //    else {
 //      res.redirect('/');
 //    }

 //  }
};

