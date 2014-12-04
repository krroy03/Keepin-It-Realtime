(function($){
  $(function(){


    function renderScores(data) {
      var scoreboard = $('#scoreboardList');
      scoreboard.find('*').remove();
      
      $.each(data.scores, function( i, val ) {
        var bar = $('<div class="bar"></div>');

        bar.append($('<h4>' + val.username + '</h4>'));
        bar.append($('<p>' + val.score + '</p>'));
        scoreboard.append(bar);

      });
    }


    var req_url;
    if (window.location.pathname === "/game/chess") {
      req_url = "/score/chess";
    }
    else if (window.location.pathname === "/game/platformer") {
      req_url = "/score/platformer";
    }

    function refreshScores() {
      console.log("Refresh");
        $.ajax({
            type: "GET",
            url : req_url,
            data : {refresh: true},
            dataType : "json",
            success: function( data ){

              console.log("Refresh data", data.scores);
              renderScores(data);
            }
        });
    }


    refreshScores();
    // causes the sendRequest function to run every 10 seconds
    window.setInterval(refreshScores, 11000);

  }); // end of document ready
})(jQuery); // end of jQuery name space