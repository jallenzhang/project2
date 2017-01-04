$(document).ready(function () {
    $("#side-nav li").not(".active").find("ul").hide();
    $("#side-nav .button").click(function () {
        if ($(this).parent("li").hasClass("active")) {
            $(this).next("ul").hide("fast");
            $(this).parent().removeClass("active").addClass("inactive").find(".expand").removeClass("expanded");
        } else {
            $(this).next("ul").show("fast");
            $(this).parent().removeClass("inactive").addClass("active").find(".expand").addClass("expanded");
        }
    });
}); 