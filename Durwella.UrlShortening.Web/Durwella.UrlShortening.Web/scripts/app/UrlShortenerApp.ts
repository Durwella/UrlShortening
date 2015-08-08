/// <reference path="../typings/servicemodel.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />

module UrlShortenerApp {
    "use strict";
    import ShortUrlResponse = Durwella.UrlShortening.Web.ServiceModel.ShortUrlResponse;
    import ShortUrlRequest = Durwella.UrlShortening.Web.ServiceModel.ShortUrlRequest;
    import ResponseStatus = Durwella.UrlShortening.Web.ServiceModel.ResponseStatus;
    import HttpPromiseCallback = angular.IHttpPromiseCallback;
    import HttpHeadersGetter = angular.IHttpHeadersGetter;
    import RequestConfig = angular.IRequestConfig;

    interface IUrlShortenerScope extends ng.IScope {
        longUrl: string;
        customPath: string;
        shortenedUrl: string;
        errorMessage: string;
        waiting: boolean;
        submitLongUrl: Function;
        unauthorized: boolean;
    }

    enum HttpStatusCodes {
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408
        //...
    }

    class UrlShortenerCtrl {

        // $inject annotation.
        // It provides $injector with information about dependencies to be injected into constructor
        // it is better to have it close to the constructor, because the parameters must match in count and type.
        // See http://docs.angularjs.org/guide/di
        static $inject = [
            "$scope",
            "$http",
            "$location"
        ];

        // dependencies are injected via AngularJS $injector
        // controller's name is registered in Application.ts and specified from ng-controller attribute in index.html
        constructor(
            private $scope: IUrlShortenerScope,
            private $http: ng.IHttpService,
            private $location: ng.ILocationService
        ) {
            $scope.submitLongUrl = () => {
                if (!$scope.longUrl)
                    return;
                $scope.shortenedUrl = null;
                $scope.errorMessage = null;
                $scope.waiting = true;
                var request = <ShortUrlRequest> { Url: $scope.longUrl, CustomPath: $scope.customPath };
                $http.post("/shorten", request)
                    .success(this.onSuccess)
                    .error(this.onError);
            };
        }

        onSuccess: HttpPromiseCallback<ShortUrlResponse> = (response: ShortUrlResponse, status: number, headers: HttpHeadersGetter, config: RequestConfig) => {
            this.$scope.shortenedUrl = response.Shortened;
            this.$scope.waiting = false;
        }

        onError: HttpPromiseCallback<any> = (response: any, status: number, headers: HttpHeadersGetter, config: RequestConfig) => {
            console.log("Error: ", response);
            var message = "Unknown Error";
            if (status === HttpStatusCodes.Unauthorized) {
                this.$scope.unauthorized = true;
                message = "Unauthorized. You must log in to create a short URL.";
            }
            if (response.hasOwnProperty("ResponseStatus")) {
                var responseStatus = <ResponseStatus>response.ResponseStatus;
                message = responseStatus.Message;
            }
            this.$scope.errorMessage = message;
            this.$scope.waiting = false;
        };
    }

    angular.module("urlShortenerApp", [])
        .controller("urlShortenerCtrl", UrlShortenerCtrl);
}