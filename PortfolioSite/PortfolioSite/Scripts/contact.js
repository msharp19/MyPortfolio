$(function () {

    var isSubmittingOrderForm = false;

    $("#send-email").on("click", function () {
        if (isSubmittingOrderForm === false) {
            isSubmittingOrderForm === true;
            var isClean = checkForEmptyFields();
            var name = $("#name-val").val();
            var email = $("#email-val").val();
            var comment = $("#comments").val();
            var data = { name: name, email: email, message: comment};
            if (isClean === true) {
                toggle();
                $.ajax({
                    type: "POST",
                    url: "../Contact/Message",
                    data: data,
                    success: function (data) {
                        if (data.Status === "Success") {
                            //clear form
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
    }

    $("#comments").on("keyup", function () {
        checkField("#comments");
    });

    $("#email-val").on("keyup", function () {
        checkField("#email-val");
    });

    $("#name-val").on("keyup", function () {
        checkField("#name-val");
    });

    function checkField(id) {
        var fieldVal = $(id).val();
        var emptyField = false;
        var parentContainer = $(id).parent().find(".validation-email");
        if ((fieldVal && fieldVal.trim().length > 0)) {
            showSuccessMarker(parentContainer);
        } else {
            showErrorMarker(parentContainer);
            emptyField = true;
        }
        return emptyField;
    }

    function checkForEmptyFields() {
        var emptyField = false;
        var parentContainer = null;
        //Make the checks
        var comment = checkField("#comments");
        var email = checkField("#email-val");
        var name = checkField("#name-val");
        emptyField = emptyField === true ? true : comment;
        emptyField = emptyField === true ? true : email;
        emptyField = emptyField === true ? true : name;
        return !emptyField;
    }

    function showSuccessMarker(parentContainer) {
        parentContainer.find(".fa-check").show();
        parentContainer.find(".fa-exclamation").hide();
    }

    function showErrorMarker(parentContainer) {
        parentContainer.find(".fa-check").hide();
        parentContainer.find(".fa-exclamation").show();
    }

    function outputWarningMessage(errMsg, title) {
        Command: toastr["warning"](errMsg, title)
    }

    function outputSuccessMessage(successMsg, title) {
        Command: toastr["success"](successMsg, title)
    }

    function outputErrorMessage(errMsg, title) {
        Command: toastr["error"](errMsg, title)
    }

});