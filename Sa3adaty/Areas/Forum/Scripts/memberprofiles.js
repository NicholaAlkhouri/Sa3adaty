$(function () {

    // Get the this week earners
    $.get(app_base + "Forum/Members/GetMemberDiscussions",
    function (data) {
        $(".thisweekleaderboard").html(data);
    });


});