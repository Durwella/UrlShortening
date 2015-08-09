/// <reference path="../typings/angular-ui-bootstrap/angular-ui-bootstrap.d.ts" />
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
    import ModalService = angular.ui.bootstrap.IModalService;
    import ModalServiceInstance = angular.ui.bootstrap.IModalServiceInstance;
    import Authenticate = Durwella.UrlShortening.Web.ServiceModel.Authenticate;

    interface IUrlShortenerScope extends ng.IScope {
        longUrl: string;
        customPath: string;
        shortenedUrl: string;
        errorMessage: string;
        waiting: boolean;
        submitLongUrl: Function;
        unauthorized: boolean;
        showLogon: () => void;
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
            "$modal",
            "$location"
        ];

        // dependencies are injected via AngularJS $injector
        // controller's name is registered in Application.ts and specified from ng-controller attribute in index.html
        constructor(
            private $scope: IUrlShortenerScope,
            private $http: ng.IHttpService,
            private $modal: ModalService,
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
            $scope.showLogon = () => this.showLogon();
        }

        onSuccess: HttpPromiseCallback<ShortUrlResponse> = (
            response: ShortUrlResponse, status: number,
            headers: HttpHeadersGetter, config: RequestConfig
        ) => {
            this.$scope.shortenedUrl = response.Shortened;
            this.$scope.waiting = false;
        }

        onError: HttpPromiseCallback<any> = (
            response: any, status: number,
            headers: HttpHeadersGetter, config: RequestConfig
        ) => {
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

        showLogon() {
            var modalInstance = this.$modal.open({
                templateUrl: '_logon.html',
                controller: 'logonCtrl'
            });
        }
    }

    interface ILogonScope extends ng.IScope {
        userName: string;
        password: string;
        errorMessage: string;
        submitLogin: () => void;
    }

    class LogonCtrl {
        static $inject = [
            "$scope",
            "$http",
            "$modalInstance"
        ];
        constructor(
            private $scope: ILogonScope,
            private $http: ng.IHttpService,
            private $modalInstance: ModalServiceInstance
            ) {
            $scope.submitLogin = () => {
                var authRequest = <Authenticate> {
                    UserName: $scope.userName,
                    Password: $scope.password
                };
                $http.post("/auth/credentials", authRequest)
                    .success((response: any) => {
                        $modalInstance.close();
                    })
                    .error(() => {
                        $scope.errorMessage = "Login failed.";
                    });
            };
        }
    }


    angular.module("urlShortenerApp", ['ui.bootstrap'])
        .controller("urlShortenerCtrl", UrlShortenerCtrl)
        .controller("logonCtrl", LogonCtrl)
    ;
}
