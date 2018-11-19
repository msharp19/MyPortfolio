$(function () {

    var isSubmittingForm = true;
    
    $("#add-comment").on("click", function () {
        var username = $("#username").val();
        var emailValue = $("#email-value").val(); 
        var commentText = $("#comment-text").val();
        var blogName = $("#BlogPostName").val();
        if (checkFieldsArePopulated()) {
            var data = { blogName: blogName, userName : username, email: emailValue, comment: commentText};
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

    function checkFieldsArePopulated() {

        return false;
    }

});