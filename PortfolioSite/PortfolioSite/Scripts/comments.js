$(function () {

    var isSubmittingForm = true;
    
    $("#add-comment").on("click", function () {
        var username = $("#name-val").val();
        var emailValue = $("#email-val").val(); 
        var commentText = $("#comments").val();
        var blogName = $("#BlogPostName").val();
        var replyId = ($("#ReplyId").val().length > 0 ? $("#ReplyId").val() : null);
        if (checkForEmptyFields()) {
            var data = {
                blogName: blogName, userName: username, email: emailValue, comment: commentText,
                CaptchaDeText: $("#CaptchaDeText").val(), CaptchaInputText: $("#CaptchaInputText").val(),
                replyId: replyId
            };
            $.ajax({
                type: "POST",
                url: "../Blog/AddComment",
                data: data,
                success: function (data) {
                    if (data.Status === "Success") {
                        outputSuccessMessage(data.Message, "Contact Recieved");
                    }
                    else {
                        outputErrorMessage(data.Message, "Captcha Failure");
                        setTimeout(function () {
                            window.location.assign(data.Url);
                        }, 1200);
                    }
                    toggle();
                },
                error: function () {
                    outputErrorMessage("Something went wrong :/", "Opps");
                    isSubmittingForm = false;
                }
            });
        }
    });

});

function reply(username, id) {
    $(".reply-panel").show();
    $(".reply-name").text(username);
    $('html, body').animate({
        scrollTop: parseInt($('.commentys-form').offset().top-65)
    }, 2000);
    $("#ReplyId").val(id);
}