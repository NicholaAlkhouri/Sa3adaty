$(function () {
    CallHome();
    LatestUsers();
    LowestPointUsers();
    LowestPointPosts();
    HighestViewedTopics();
    LatestNews();
    TodaysTopics();
});

function LatestUsers() {
    $.post(app_base + "ForumAdmin/Dashboard/LatestUsers",
    function (data) {
        $(".dashboardlatestusers").html(data);
    });
}

function LowestPointUsers() {
    $.post(app_base + "ForumAdmin/Dashboard/LowestPointUsers",
    function (data) {
        $(".dashboardlowestpointusers").html(data);
    });
}

function LowestPointPosts() {
    $.post(app_base + "ForumAdmin/Dashboard/LowestPointPosts",
    function (data) {
        $(".dashboardlowestpointposts").html(data);
    });
}

function TodaysTopics() {
    $.post(app_base + "ForumAdmin/Dashboard/TodaysTopics",
    function (data) {
        $(".dashboardtodaystopics").html(data);
    });
}

function HighestViewedTopics() {
    $.post(app_base + "ForumAdmin/Dashboard/HighestViewedTopics",
    function (data) {
        $(".dashboardhighestviewedtopics").html(data);
    });
}

function LatestNews() {
    $.post(app_base + "ForumAdmin/Dashboard/MvcForumLatestNews",
    function (data) {
        $(".mvcforumlatestnews").html(data);
    });
}
function CallHome() {
    $.post(app_base + "ForumAdmin/Admin/Aptitude", function (data) {
    });
}