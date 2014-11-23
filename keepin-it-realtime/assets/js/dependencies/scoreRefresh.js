(function($){
  $(function(){


    function renderScores(data) {
      var scoreboard = $('#scoreboard');
      scoreboard.find('*').not('h1').remove();
      
      $.each(data.scores, function( i, val ) {
        var bar = $('<div class="bar"></div>');

        bar.append($('<h4>' + val.username + '</h4>'));
        bar.append($('<p>' + val.score + '</p>'));
        scoreboard.append(bar);

      });
    }

    function refreshScores() {
      console.log("Refresh");
        $.ajax({
            type: "GET",
            url : "/score/showAll",
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