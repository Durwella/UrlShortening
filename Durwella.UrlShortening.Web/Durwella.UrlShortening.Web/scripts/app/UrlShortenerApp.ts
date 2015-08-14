/// <reference path="../typings/servicemodel.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../typings/angular-ui-bootstrap/angular-ui-bootstrap.d.ts" />

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
    import AuthenticateResponse = Durwella.UrlShortening.Web.ServiceModel.AuthenticateResponse;


    export class UrlShortenerForm {
        longUrl: string;
        customPath: string;
    }

    interface IUrlShortenerScope extends ng.IScope {
        form: UrlShortenerForm;
        shortenedUrl: string;
        errorMessage: string;
        waiting: boolean;
        isAuthenticated: boolean;
        submitLongUrl: () => void;
        showLogin: () => void;
        logout: () => void;
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
            "$attrs",
            "$location"
        ];

        // dependencies are injected via AngularJS $injector
        // controller's name is registered in Application.ts and specified from ng-controller attribute in index.html
        constructor(
            private $scope: IUrlShortenerScope,
            private $http: ng.IHttpService,
            private $modal: ModalService,
            private $attrs: ng.IAttributes,
            private $location: ng.ILocationService
        ) {
            $scope.isAuthenticated = $attrs["startAuthenticated"] === "True";
            $scope.form = new UrlShortenerForm();
            $scope.form.longUrl = $location.search().longUrl;
            $scope.submitLongUrl = () => {
                var form = $scope.form;
                if (!form.longUrl)
                    return;
                $location.search("longUrl", form.longUrl);
                $scope.shortenedUrl = null;
                $scope.errorMessage = null;
                $scope.waiting = true;
                var request = <ShortUrlRequest> { Url: form.longUrl, CustomPath: form.customPath };
                $http.post("/shorten", request)
                    .success(this.onSuccess)
                    .error(this.onError);
            };
            $scope.showLogin = () => this.showLogin();
            $scope.logout = () => this.logout();
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
            if (status === HttpStatusCodes.Unauthorized) {
                this.$scope.errorMessage =
                    "Unauthorized. You must log in to create a short URL.";
                this.showLogin();
            }
            else
                this.$scope.errorMessage =
                    UrlShortenerCtrl.getMessageFromResponseStatus(response);
            this.$scope.waiting = false;
        };

        showLogin() {
            var modal = this.$modal.open({
                templateUrl: "login",
                controller: "loginCtrl"
            });
            modal.result.then(() => {
                this.$scope.isAuthenticated = true;
                this.$scope.submitLongUrl();
            });
        }

        logout() {
            this.$http.get("/auth/logout")
                .success(() => this.$scope.isAuthenticated = false);
        }

        static getMessageFromResponseStatus(response: any) {
            var message = "Unknown Error";
            if (response.hasOwnProperty("ResponseStatus")) {
                var responseStatus = <ResponseStatus>response.ResponseStatus;
                message = responseStatus.Message;
            }
            return message;
        }
    }

    interface ILoginScope extends ng.IScope {
        userName: string;
        password: string;
        errorMessage: string;
        submitLogin: () => void;
    }

    class LoginCtrl {
        static $inject = [
            "$scope",
            "$http",
            "$modalInstance"
        ];
        constructor(
            private $scope: ILoginScope,
            private $http: ng.IHttpService,
            private $modalInstance: ModalServiceInstance
            ) {
            $scope.submitLogin = () => {
                this.$scope.errorMessage = null;
                var authRequest = <Authenticate> {
                    UserName: $scope.userName,
                    Password: $scope.password
                };
                $http.post("/auth/credentials", authRequest)
                    .success((response: AuthenticateResponse) => {
                        $modalInstance.close(response.UserName);
                    })
                    .error((response: any) => {
                        this.$scope.errorMessage = 
                            UrlShortenerCtrl.getMessageFromResponseStatus(response);
                    });
            };
        }
    }


    angular.module("urlShortenerApp", ['ui.bootstrap'])
        .controller("urlShortenerCtrl", UrlShortenerCtrl)
        .controller("loginCtrl", LoginCtrl)
    ;
}
