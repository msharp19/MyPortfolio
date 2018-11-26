(function () {
    var canvas = document.querySelector('#paint');
    var ctx = canvas.getContext('2d');

    var sketch = document.querySelector('#sketch');
    var sketch_style = getComputedStyle(sketch);
    canvas.width = parseInt(sketch_style.getPropertyValue('width'));
    canvas.height = parseInt(sketch_style.getPropertyValue('height'));

    var mouse = { x: 0, y: 0 };
    var last_mouse = { x: 0, y: 0 };

    /* Mouse Capturing Work */
    canvas.addEventListener('mousemove', function (e) {
        last_mouse.x = mouse.x;
        last_mouse.y = mouse.y;

        mouse.x = e.pageX - this.offsetLeft;
        mouse.y = e.pageY - this.offsetTop;
    }, false);


    setupCanvas(ctx);

    canvas.addEventListener('mousedown', function (e) {
        canvas.addEventListener('mousemove', onPaint, false);
    }, false);

    canvas.addEventListener('mouseup', function () {
        canvas.removeEventListener('mousemove', onPaint, false);
    }, false);

    var onPaint = function () {
        ctx.beginPath();
        ctx.moveTo(last_mouse.x, last_mouse.y);
        ctx.lineTo(mouse.x, mouse.y);
        ctx.closePath();
        ctx.stroke();
    };

}());

function setupCanvas(ctx) {
    /* Drawing on Paint App */
    ctx.lineWidth = 20;
    ctx.lineJoin = 'round';
    ctx.lineCap = 'round';
    ctx.strokeStyle = 'black';
}

$("#clear-canvas").on("click", function () {
    var canvas = $('#paint')[0]; 
    canvas.width = canvas.width;
    var ctx = canvas.getContext('2d');
    setupCanvas(ctx);
    $(".predict-box-inner").text("");
});

function getBase64Image() {
    var canvas = $('#paint')[0];
    var ctx = canvas.getContext('2d');
    var imgData = ctx.getImageData(0, 0, 150, 150);
    var dataURL = canvas.toDataURL("image/png");
    return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
}

$("#predict").on("click", function () {
    var base64 = getBase64Image();
    var data = { imageRaw: base64 };
    $.ajax({
        type: "POST",
        url: "../Blog/PredictMnist",
        data: data,
        success: function (data) {
            if (data.Status === "Success") {
                outputSuccessMessage(data.Message, "Success");
                $(".predict-box-inner").text(data.Label);
            }
            else {
                outputErrorMessage(data.Message, "Error");
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
});
