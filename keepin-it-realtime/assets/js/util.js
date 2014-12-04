// Post status message to a public room
function postStatusMessage(roomName, message) {

  var div = $('<div style="text-align: center">----- '+message+' -----</div>');
  $('#'+roomName).append(div);

}