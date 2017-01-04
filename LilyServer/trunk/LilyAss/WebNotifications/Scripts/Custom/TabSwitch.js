$(document).ready(function () {
    // When a link is clicked
    $("a.tab").click(function () {

        // switch all tabs off
        $(".active").removeClass("active");

        // switch this tab on
        $(this).addClass("active");

        // slide all content up
        $(".tabcontent").slideUp();

        // slide this content up
        var content_show = $(this).attr("title");
        if (is_first && content_show == content_flag) {
            is_first = false;
            printFlash();
        }

        $("#" + content_show).slideDown();
    });
}); 