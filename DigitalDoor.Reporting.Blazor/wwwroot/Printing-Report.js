let newWebWindow;
export const PrintReports = {
    AddCssToPage: (width, height) => {
        const myContainer = document.getElementById('pdf-styles');
        if (myContainer == null) {
            let print = document.createElement('link');
            print.id = "pdf-styles";
            print.rel = 'stylesheet';
            print.href = './_content/DigitalDoor.Reporting.Blazor/style.css';
            document.getElementsByTagName('head')[0].appendChild(print);
            window.addEventListener('beforeunload', function (e) {
                //remove css used
                try {
                    const myContainer = document.getElementById('pdf-styles');
                    if (myContainer == null) {
                        myContainer.remove();
                    }
                } catch (e) {
                    console.info(e);
                }
            });
        }
    },
    AddJavascriptsToPage: () => {
        let jspdf = document.getElementById('pdf-javascripts-jspdf')
        if (jspdf == null) {
            jspdf = document.createElement('script');
            jspdf.type = 'text/javascript';
            jspdf.src = "https://cdnjs.cloudflare.com/ajax/libs/jspdf/1.5.3/jspdf.min.js";
            jspdf.id = "pdf-javascripts-jspdf";
            document.getElementsByTagName('head')[0].appendChild(jspdf);
        }
        let html2canvas = document.getElementById('pdf-javascripts-html2canvas')
        if (html2canvas == null) {
            html2canvas = document.createElement('script');
            html2canvas.type = 'text/javascript';
            html2canvas.src = "https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.js";
            html2canvas.id = "pdf-javascripts-html2canvas";
            document.getElementsByTagName('head')[0].appendChild(html2canvas);
        }
        window.addEventListener('beforeunload', function (e) {
            //remove javascripts used
            try {
                let jspdf = document.getElementById('pdf-javascripts-jspdf');
                if (jspdf == null) {
                    jspdf.remove();
                }
                let html2canvas = document.getElementById('pdf-javascripts-html2canvas');
                if (html2canvas == null) {
                    html2canvas.remove();
                }
            } catch (e) {
                console.info(e);
            }
        });
    },
    CreatePdf: (wrapperId, fileName, returnBytes, orientation, size) => {
        var response = {
            Result: true,
            Base64String: '',
            Message: ''
        }       
        const getCanvasContent = new Promise(function (result, error) {
            try {
                var PdfImages = [{ 'base': null }];
                var pageContainers = document.querySelectorAll(`#${wrapperId} .main--container`);
                pageContainers.forEach(function (key, index) {
                    try {
                        html2canvas(pageContainers[index]).then(function (canvas) {
                            try {
                                var data = canvas.toDataURL("img/JPG");
                                PdfImages.push({ 'base': data });
                                if (index == pageContainers.length - 1) {
                                    result(PdfImages);
                                }
                            } catch (e) {
                                response.Result = false;
                                response.Message = e.message;
                                error(response);
                            }
                        });
                    } catch (e) {
                        response.Result = false;
                        response.Message = e.message;
                        error(response);
                    }
                });
            }
            catch (e) {
                response.Result = false;
                response.Message = e.message;
                error(response);
            }
        });        

        const createPdf = function (pages) {
            return new Promise(function (result, error) {
                try {
                    var options = {
                        orientation: orientation,  // p, l
                        unit: 'mm',
                        format: size,   //a4 b3 [1231,1212]
                        precision: 1,
                        compress: true
                    }
                    var doc = new jsPDF(options);
                    var total = pages.length;
                    var j = 1;
                    do {
                        doc.addImage(pages[j].base, "JPEG", 0, 0, 0, 0, "a" + j, "FAST");
                        j++
                        if (j < total) doc.addPage();
                    } while (j < total);

                    var blob = doc.output('blob');
                    var reader = new FileReader();
                    reader.readAsDataURL(blob);
                    response.Result = true;
                    response.Message = "Printed!";
                    reader.onloadend = function () {
                        var base64data = reader.result;
                        response.Base64String = base64data;

                        if (!returnBytes) {
                            doc.save(fileName);
                        }
                        result(response); 
                    }
                } catch (e) {
                    response.Result = false;
                    response.Message = e.message;
                    error(response);
                }
            });
        }
        return getCanvasContent.then((pages) => createPdf(pages).catch((error) => error)).catch((error) => error);
    }
}