(function($){
  $(function(){


    function renderScores(data) {
      $.each(data, function( i, val ) {
        var scoreboard = $('#scoreboard');
        var bar = $('<div class="bar"></div>');

        scoreboard.find('*').not('h1').remove();
        bar.append($('<h4>' + val[0].username + '</h4>'));
        bar.append($('<p>' + val[0].score + '</p>'));
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

              console.log("Refresh data", data);
              renderScores(data);
            }
        });
    }

    refreshScores();
    // causes the sendRequest function to run every 10 seconds
    window.setInterval(refreshScores, 11000);

  }); // end of document ready
})(jQuery); // end of jQuery name space