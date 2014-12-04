/**
 * Route Mappings
 * (sails.config.routes)
 *
 * Your routes map URLs to views and controllers.
 *
 * If Sails receives a URL that doesn't match any of the routes below,
 * it will check for matching files (images, scripts, stylesheets, etc.)
 * in your assets directory.  e.g. `http://localhost:1337/images/foo.jpg`
 * might match an image file: `/assets/images/foo.jpg`
 *
 * Finally, if those don't match either, the default 404 handler is triggered.
 * See `api/responses/notFound.js` to adjust your app's 404 logic.
 *
 * Note: Sails doesn't ACTUALLY serve stuff from `assets`-- the default Gruntfile in Sails copies
 * flat files from `assets` to `.tmp/public`.  This allows you to do things like compile LESS or
 * CoffeeScript for the front-end.
 *
 * For more information on configuring custom routes, check out:
 * http://sailsjs.org/#/documentation/concepts/Routes/RouteTargetSyntax.html
 */

module.exports.routes = {

  /***************************************************************************
  *                                                                          *
  * Make the view located at `views/homepage.ejs` (or `views/homepage.jade`, *
  * etc. depending on your default view engine) your home page.              *
  *                                                                          *
  * (Alternatively, remove this and add an `index.html` file in your         *
  * `assets` directory)                                                      *
  *                                                                          *
  ***************************************************************************/

  '/': {
    controller: 'home'
  },
  '/user':{
    controller: 'home'
  },
  '/login': {
    controller: 'AuthController',
    action: 'login'
  },
  '/process': {
    controller: 'AuthController',
    action: 'process'
  },
  '/logout': {
    controller: 'AuthController',
    action: 'logout'
  },
  '/game': {
    controller: 'ScoreController',
    action: 'chooseGame'
  },
  '/game/chess': {
    controller: 'ScoreController',
    action: 'chess'
  },
  '/game/shooter': {
    controller: 'ScoreController',
    action: 'shooter'
  },
  '/game/platformer': {
    controller: 'ScoreController',
    action: 'platformer'
  },
  'post /addFriend/:id':{
    controller: 'UserController',
    action: 'addFriend'
  },
  'get /message/:id':{
    controller: 'UserController',
    action: 'viewMessages'
  },
  'post /message/:id':{
    controller: 'UserController',
    action: 'sendMessage'
  },

  'make /room/:roomName/users':{
    controller:'RoomController',
    action: 'create'
  },

  'post /room/:roomId/users':{
    controller:'RoomController',
    action: 'join'
  },
  'delete /room/:roomId/users':{
    controller: 'RoomController',
    action: 'leave'
  },
  '/edit': {
    controller: 'UserController',
    action: 'editProfile'
  },
  '/edit/process': {
    controller: 'UserController',
    action: 'updateProfile'
  },
  /***************************************************************************
  *                                                                          *
  * Custom routes here...                                                    *
  *                                                                          *
  *  If a request to a URL doesn't match any of the custom routes above, it  *
  * is matched against Sails route blueprints. See `config/blueprints.js`    *
  * for configuration options and examples.                                  *
  *                                                                          *
  ***************************************************************************/

};
