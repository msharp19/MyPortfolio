$(function () {
    var isSubmittingOrderForm = false;
    $("#send-email").on("click", function () {
        if (isSubmittingOrderForm === false) {
            isSubmittingOrderForm === true;
            var isClean = checkForEmptyFields();
            var name = $("#name-val").val();
            var email = $("#email-val").val();
            var comment = $("#comments").val();
            var data = { name: name, email: email, message: comment, CaptchaDeText: $("#CaptchaDeText").val(), CaptchaInputText: $("#CaptchaInputText").val() };
            if (isClean === true) {
                toggle();
                $.ajax({
                    type: "POST",
                    url: "../Contact/Message",
                    data: data,
                    success: function (data) {
                        if (data.Status === "Success") {
                            outputSuccessMessage(data.Message, "Contact Recieved");
                            setTimeout(function () {
                                window.location.assign(data.Url);
                            }, 1200);
                        }
                        else {
                            outputErrorMessage(data.Message, "Captcha Failure");
                            toggle();
                        }
                    },
                    error: function () {
                        outputErrorMessage("Something went wrong :/", "Opps");
                        isSubmittingOrderForm = false;
                        toggle();
                    }
                });
            } else {
                isSubmittingOrderForm === false;
                outputWarningMessage("Please fill in all fields on form", "Missing Data");
            }
        } else {
            outputWarningMessage("Please wait until this message has sent", "Sending..");
        }
    });

    function toggle() {
        $("#email-load").toggleClass("none");
        $("#email-load").toggleClass("inline");
        $("#send-email").toggleClass("disabled-btn");
    }

});