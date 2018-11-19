$(function () {
    $(".work-item").click(function (e) {
        e.preventDefault();
        if ($(this).hasClass("disabled"))
            return false;
        else
            window.location.href = $(this).attr("href");
    });

    $("#menu-button").on("click", function (e) {
        if ($(this).hasClass("open")) {
            $(".work-item").each(function () {
                $(this).removeClass('disabled');
            });
        } else {
            $(".work-item").each(function () {
                $(this).addClass('disabled');
            });
        }
    });

    $(".effect-oscar").hover(
        function() {
            $(this).find(".post-date").fadeOut('slow');
        }, function() {
            $(this).find(".post-date").fadeIn('slow');
        });
});