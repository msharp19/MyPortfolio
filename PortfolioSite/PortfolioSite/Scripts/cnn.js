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
            }
            toggle();
        },
        error: function () {
            outputErrorMessage("Something went wrong :/", "Opps");
            isSubmittingForm = false;
        }
    });
});

window.onload = function () {

    //Grid image
    function drawImage(id, imgId, x, y) {
        var c = document.getElementById(id);
        var ctx = c.getContext("2d");
        var img = document.getElementById(imgId);
        ctx.drawImage(img, 0, 0, x, y);
    }

    function fillGrid() {
        var canvas = document.getElementById("rep-img-hidden");
        var ctx = canvas.getContext("2d");
        var imgData = ctx.getImageData(0, 0, canvas.width, canvas.height);
        var data = imgData.data;
        var tableGrid = $(".number-grid");
        var currentRow = "<tr>";
        var currentColumnCount = 0;
        var reset = false;
        for (var i = 0; i < data.length; i += 4) {
            var red = data[i];
            var green = data[i + 1];
            var blue = data[i + 2];
            var alpha = data[i + 3];
            var avg = Math.ceil((green + blue + red) / 3);

            if (currentColumnCount >= 19) {
                currentRow += "</tr>";
                tableGrid.append(currentRow);
                currentRow = "<tr>";
                currentColumnCount = 0;
                reset = true;
            }
            else {
                reset = false;
                currentRow += ("<td>" + avg + "</td>");
            }
            if(!reset) currentColumnCount++;
        }
    }
    drawImage("rep-img-hidden", "grid-img-hidden", 20,20);
    fillGrid();
};
