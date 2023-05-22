
export const PrintReports = {
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
        let response = {
            Result: true,
            Base64String: '',
            Message: '',
            Html: ''
        }
        const rawHtml = function () {
            let content = document.querySelectorAll(`#${wrapperId} .main--container`);
            if (content.length > 0) {
                return content[0].innerHTML;
            }
            else {
                return '';
            }
        }

        response.Html = rawHtml;
        const getCanvasContent = new Promise(function (result, error) {
            try {
                let pageContainers = document.querySelectorAll(`#${wrapperId} .main--container`);
                let validContainers = Array.from(pageContainers);
                let options = {
                    scale: 3,
                    backgroundColor: null,
                };

                const filterContainersToPrint = validContainers.filter(items => {
                    response.Message = "No document found!";
                    if (items.childNodes[1].childNodes.length == 0) {
                        return null;
                    } else if (items.childNodes[1].childNodes.length > 0)  return items;
                    else {
                        return null;
                    }
                });

                let PdfImages = [];
                if (filterContainersToPrint) {
                    filterContainersToPrint.forEach(function (key, index) {
                        try {
                            html2canvas(filterContainersToPrint[index], options).then(function (canvas) {
                                try {
                                    var data = canvas.toDataURL("img/png");
                                    PdfImages.push({ 'page': index, 'base': data });
                                    let containerCount = filterContainersToPrint.length;
                                    if (PdfImages.length == containerCount) {
                                        PdfImages.sort((a, b) => a.page - b.page)
                                        result(PdfImages);
                                    }
                                } catch (e) {
                                    response.Result = false;
                                    response.Message = 'HTML2Canvas Exception. Check console warning.';
                                    console.warn(e);
                                    error(response);
                                }
                            });
                        } catch (e) {
                            response.Result = false;
                            response.Message = 'HTML2Canvas Exception. Check console warning.';
                            console.warn(e);
                            error(response);
                        }
                    });
                }
                else {
                    response.Result = false;
                    error(response);
                }
            }
            catch (e) {
                response.Result = false;
                response.Message = e.message;
                console.warn(e);
                error(response);
            }
        });
        const createPdf = function (pages) {
            return new Promise(function (result, error) {
                try {
                    let options = {
                        orientation: orientation,  // p, l
                        unit: 'mm',
                        format: size,   //a4 b3 [1231,1212]
                        precision: 1,
                        compress: true
                    }
                    let doc = new jsPDF(options);
                    doc.internal.scaleFactor = 30;
                    let pdfInternals = doc.internal,
                        pdfPageSize = pdfInternals.pageSize,
                        pdfPageWidth = pdfPageSize.width,
                        pdfPageHeight = pdfPageSize.height;

                    let total = pages.length;
                    let j = 0;

                    do {
                        doc.addImage(pages[j].base, "png", 0, 0, pdfPageWidth, pdfPageHeight, "a" + j);
                        j++
                        if (j <= total - 1) doc.addPage();
                    } while (j < total);

                    let blob = doc.output('blob');
                    let reader = new FileReader();
                    reader.readAsDataURL(blob);
                    response.Result = true;
                    response.Message = "Printed!";
                    reader.onloadend = function () {
                        let base64data = reader.result;
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
        return getCanvasContent.then(pages => createPdf(pages).catch(error => error)).catch(error => error);
    },
    GetHtml: (wrapperId) => {
        return new Promise(function (result, error) {
            let response = {
                Result: true,
                Base64String: '',
                Message: '',
                Html: ''
            }
            try {
                let content = document.querySelectorAll(`#${wrapperId} .main--container`);
                if (content.length > 0) {
                    response.Html = content[0].innerHTML;
                }
                else {
                    response.Html = '';
                }
                response.Result = true;
                response.Message = 'Done!. All good!'
                result(response);
            } catch (e) {
                response.Result = false;
                response.Message = e.message;
                error(response);
            }
        });
    }
}