import ko from 'knockout';
import auth from './auth';

class Http {
    constructor() {
    }

    get(url, cb) {
        this.ajax(url, cb, 'GET');
    }

    post(url, data, cb) {
        this.ajax(url, cb, 'POST', { data: data });
    }

    ajax(url, cb, method, opt) {
        var urlComponents = url.split("?");
        urlComponents[0] = urlComponents[0] + '/' + auth.currentUserId();
        url = urlComponents.join('?');

        var ajaxParams = {
            method: method,
            headers: { 'Authorization' : 'Bearer ' + sessionStorage.getItem('token')},
            url: url
        };

        if (opt && opt.data) {
            $.extend(ajaxParams, { data: opt.data });
        } 
        
        $.ajax(ajaxParams).done(cb);
    }

}

var http = new Http();

export default http;