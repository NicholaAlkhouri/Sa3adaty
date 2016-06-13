$(function () {

    // Get the this week earners
    $.get(app_base + "Forum/Snippets/GetThisWeeksTopEarners",
    function (data) {
        $(".thisweekleaderboard").html(data);
    });

    // Get the this week earners
    $.get(app_base + "Forum/Snippets/GetThisYearsTopEarners",
    function (data) {
        $(".alltimeleaderboard").html(data);
    });

});