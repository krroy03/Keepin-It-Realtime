// Create the HTML to hold a public, multi-user chat room
function createPublicRoom(room) {

  // Get the ID of the HTML element for this public room, if there is one
  var roomName = 'public-room-'+room.id;

  // If HTML for the room already exists, return.
  if ($('#'+roomName).length) {
    return;
  }

  // Create a new div to contain the room
  var roomDiv = $('<div id="'+roomName+'"></div>');

  // Create the HTML for the room
  var roomHTML = '<h2>Chat room &ldquo;'+room.name+'&rdquo; <button class="btn btn-primary" id="leave-room-button-'+room.id+'">Leave Room</button></h2>\n' +
                 '<div id="room-messages-'+room.id+'" style="width: 50%; height: 150px; overflow: auto; border: solid 1px #666; padding: 5px; margin: 5px"></div>'+
                 '<input class="form-control" id="room-message-'+room.id+'"/> <button class="btn btn-primary" id="room-button-'+room.id+'">Send message</button">';

  roomDiv.html(roomHTML);

  // Add the room to the private conversation area
  $('#rooms').append(roomDiv);

  // Hook up the "send message" button
  $('#room-button-'+room.id).click(onClickSendPublicMessage);
  $('#room-message-'+room.id).keydown(function(e) {
    if(e.keyCode==13){
       $('#room-button-'+room.id).trigger('click');
    }
  });

  // Hook up the "leave room" button
  $('#leave-room-button-'+room.id).click(onClickLeaveRoom);

}

// Callback for when the user clicks the "Send message" button in a public room
function onClickSendPublicMessage(e) {

  // Get the button that was pressed
  var button = e.currentTarget;

  // Get the ID of the user we want to send to
  var roomId = button.id.split('-')[2];

  // Get the message to send
  var message = $('#room-message-'+roomId).val();
  $('#room-message-'+roomId).val("");

  // Add this message to the room
  $.ajax({
    type: "GET",
    url : "/user/current_user_object",
    data : {refresh: true},
    dataType : "json",
    success: function( user ){
      var user_id = user['user']['id'];
      var user_name = user['user']['username'];
      addMessageToChatRoom(user_id, user_name, roomId, message);
      // Send the message
      io.socket.post('/chat/public', {room: roomId, msg: message});
    }
  });

}

// Add HTML for a new message in a public room
function addMessageToChatRoom(senderId, sender_name, roomId, message) {

  $.ajax({
    type: "GET",
    url : "/user/current_user_object",
    data : {refresh: true},
    dataType : "json",
    success: function( user ){



      var roomName = 'room-messages-' + roomId;

      if (senderId === 0) {
        return postStatusMessage(roomName, message);
      }

      var fromMe = senderId == user['user']['id'];

      var senderName = fromMe ? "You" : sender_name;
      var justify = fromMe ? 'right' : 'left';

      var div = $('<div style="text-align:'+justify+'"></div>');
      div.html('<strong>'+senderName+'</strong>: '+message);
      $('#'+roomName).append(div);
      $('#'+roomName).scrollTop($('#'+roomName)[0].scrollHeight);

    }
  });

}

// Handle an incoming public message from the server.
function receiveRoomMessage(data) {

  var sender = data.from;
  var room = data.room;

  // Create a room for this message if one doesn't exist
  createPublicRoom(room);

  // Add a message to the room
  addMessageToChatRoom(sender.id, sender.username, room.id, data.msg);

}

// Join the room currently selected in the list
function joinRoom() {

  // Get the room list
  var select = $('#rooms-list');

  // Make sure a room is selected in the list
  if (select.val() === null) {
    return alert('Please select a room to join.');
  }

  // Get the room's name from the text of the option in the <select>
  var roomName = $('option:selected', select).attr('data-name');
  var roomId = select.val();

  // Create the room HTML
  createPublicRoom({id:roomId, name:roomName});

  // Join the room
  $.ajax({
    type: "GET",
    url : "/user/current_user_object",
    data : {refresh: true},
    dataType : "json",
    success: function( user ){
      var user_id = user['user']['id'];
      var user_name = user['user']['username'];
      io.socket.post('/room/'+roomId+'/users', {id: user_id, name: user_name});
    }
  });

  // Update the room user count
  increaseRoomCount(roomId);

}

// Handle the user clicking the "Leave Room" button for a public room
function onClickLeaveRoom(e) {

  // Get the button that was pressed
  var button = e.currentTarget;

  // Get the ID of the user we want to send to
  var roomId = button.id.split('-')[3];

  // Remove the room from the page
  $('#public-room-'+roomId).remove();

  // Call the server to leave the room
  $.ajax({
    type: "GET",
    url : "/user/current_user_object",
    data : {refresh: true},
    dataType : "json",
    success: function( user ){
      var user_id = user['user']['id'];
      var user_name = user['user']['username'];
      io.socket.delete('/room/'+roomId+'/users', {id: user_id, name: user_name});
    }
  });

  // Update the room user count
  decreaseRoomCount(roomId);

}