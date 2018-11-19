$(function () {
    var off = true;
    $("#menu-button").on("click", function () {
        if (off) {
            off = false;
            $(".work-item").each(function () {
                $(this).addClass("disabled-link");
            });
        } else {
            off = true;
            $(".work-item").each(function () {
                $(this).removeClass("disabled-link");
            });
        }
    });
});