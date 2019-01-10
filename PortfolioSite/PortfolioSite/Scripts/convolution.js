$(document).ready(function(){

    $("#Kernels").change( function () {
        currentLimit = parseInt($("#Kernels option:selected").val());
        $(".kernal-table tr").each(function (idx) {
            $(this).find("td").each(function (jdx) {
                if (idx < currentLimit && jdx < currentLimit) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });
    });

    $(".kernal-tb").on("keyup", function (event) {
        var key = event.key;
        if (key !== "-" && key !== ".") {
            if (key && key.match(/^[0-9]+$/) === null) {
                $(this).val($(this).val().replace(key, ""));
            }
            else if (key && key.match(/^[0-9]+$/) !== null ) {
                var currentValue = Number($(this).val());
                if (currentValue > 255) {
                    currentValue = 255;
                } else if (currentValue < -255) {
                    currentValue = -255;
                }

                if (key !== "." && currentValue) $(this).val(currentValue);
            }
        }        
    });

    var thumbs = jQuery('#thumbnails').slippry({
        slippryWrapper: '<div class="slippry_box thumbnails" />',
        transition: 'horizontal',
        pager: false,
        auto: false,
        onSlideBefore: function (el, index_old, index_new) {
            jQuery('.thumbs a img').removeClass('active');
            jQuery('img', jQuery('.thumbs a')[index_new]).addClass('active');
        }
    });

    jQuery('.thumbs a').click(function () {
        thumbs.goToSlide($(this).data('slide'));
        return false;
    });

    //This is the mask size - updated when changes ie. 3x3 => 3, 5x5 => 5.. etc.
    var currentLimit = 3;
    var canvas = document.getElementById("outputCanvas");
    var image = new MarvinImage();
    //Mask as flat array
    var completeMask = [];
    //Either side of the current pixel
    var eitherSide = 0;

    //Event for running the convolution
    $("#run-convolution-btn").click(function () {
        //Build complete mask into flat array (from matrix)
        finalizeMask();
        //Create new image (clear)
        image = new MarvinImage();
        //Get the selected image url
        var url = $("#thumbnails .sy-active").find("img").attr("src");
        //Load image and run convolution
        image.load(url, runKernelOnImage);
    });

    //Builds a flat array from the 2d matrix
    function finalizeMask() {
        completeMask = [];
        $(".kernal-table tr").each(function (idx) {
            $(this).find("td").each(function (jdx) {
                if (idx < currentLimit && jdx < currentLimit) {
                    var tbVal = $(this).find(".kernal-tb").val();
                    completeMask.push(Number(tbVal));
                }
            });
        });
    }

    //Runs the kernel (just been finalized) across the image
    function runKernelOnImage() {
        //Resize the image to a bit smaller (makes things quicker)
        Marvin.scale(image.clone(), image, 800);
        //Find how far we go either side of the selected pixal ie. 3 = 1, 5 = 2, 7 = 3... etc.
        eitherSide = Math.floor(currentLimit / 2);
        //Get the image width & height
        var imgHeight = image.getHeight();
        var imgWidth = image.getWidth();
        //Create new image to add the updated pixals to
        var newImage = new MarvinImage(imgWidth, imgHeight);
        for (var y = 0; y < imgHeight; y++) {
            for (var x = 0; x < imgWidth; x++) {
                //If we are past and within the bounds of the either side. If we are on pixal 0,0 and the either side is 1 then this pixal cannot be used in convolution.
                //This also means that for every scale of the mask, a additional pixel border is added to the image. I have left this in but this can also be removed.
                if (y >= eitherSide && x >= eitherSide && y < (imgHeight - eitherSide) && x < (imgWidth - eitherSide)) {
                    //Use the kernel on the current position
                    var pixel = useKernel(x, y);
                    //Set the resulting pixel in the new image
                    newImage.setIntColor(x, y, pixel.red, pixel.blue, pixel.green);
                }
            }
        }
        //Draw the new image onto a canvas in the webpage
        newImage.draw(canvas);
    }

    //Use the kernel on the current pixel
    function useKernel(x, y) {
        //Init the x & y position to start of matrix (from image) - (0,0) relative to position ie. 3x3 at pixel position (4,5) => (3,4)
        var tmpX = x - eitherSide;
        var tmpY = y - eitherSide;
        //Set the flat count at 0 - this is used for the mask
        var flatCounter = 0;
        //Create output pixel
        var pixel = { red: 0, blue: 0, green: 0 };
        //As we are are at (0,0) (the masks position on the image) we iterate to the max mask size ie. 3x3 (1,1) = range{(0,0) => (2,2)}
        for (var i = 0; i < currentLimit; i++) {
            //Update y 
            tmpY = tmpY + i;
            for (var j = 0; j < currentLimit; j++) {
                //Update x
                tmpX = tmpX + j;
                //Get the pixel
                var red = image.getIntComponent0(tmpX, tmpY);
                var green = image.getIntComponent1(tmpX, tmpY);
                var blue = image.getIntComponent2(tmpX, tmpY);
                //Add them to the pixel
                pixel.red += (red * completeMask[flatCounter]);
                pixel.blue += (blue * completeMask[flatCounter]);
                pixel.green += (green * completeMask[flatCounter]);
                flatCounter++;
            }
        }
        //Aggregate the values from the mask moving over the image into one pixel value and return
        return pixel;
     }

    function resize() {
        $("canvas").outerHeight($(window).height() - $("canvas").offset().top - Math.abs($("canvas").outerHeight(true) - $("canvas").outerHeight()));
    }

    $(document).ready(function () {
        resize();
        $(window).on("resize", function () {
            resize();
        });
    });

});