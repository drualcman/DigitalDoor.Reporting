
export const PrintReports = {

     SaveAsFile:(filename, data) => {
        const link = document.createElement("a");
        link.href = "data:application/octet-stream;base64," + data;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
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