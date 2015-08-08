/* Options:
Date: 2015-08-08 12:32:38
Version: 1
BaseUrl: http://localhost:4257

//GlobalNamespace: 
//MakePropertiesOptional: True
//AddServiceStackTypes: True
//AddResponseStatus: False
//AddImplicitVersion: 
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/


declare module Durwella.UrlShortening.Web.ServiceModel {

    interface IReturnVoid {
    }

    interface IReturn<T> {
    }

    // @DataContract
    interface ResponseStatus {
        // @DataMember(Order=1)
        ErrorCode?: string;

        // @DataMember(Order=2)
        Message?: string;

        // @DataMember(Order=3)
        StackTrace?: string;

        // @DataMember(Order=4)
        Errors?: ResponseError[];
    }

    // @DataContract
    interface ResponseError {
        // @DataMember(Order=1, EmitDefaultValue=false)
        ErrorCode?: string;

        // @DataMember(Order=2, EmitDefaultValue=false)
        FieldName?: string;

        // @DataMember(Order=3, EmitDefaultValue=false)
        Message?: string;
    }

    interface HelloResponse {
        Result?: string;
    }

    interface ShortUrlResponse {
        Shortened?: string;
    }

    // @DataContract
    interface AuthenticateResponse {
        // @DataMember(Order=1)
        UserId?: string;

        // @DataMember(Order=2)
        SessionId?: string;

        // @DataMember(Order=3)
        UserName?: string;

        // @DataMember(Order=4)
        DisplayName?: string;

        // @DataMember(Order=5)
        ReferrerUrl?: string;

        // @DataMember(Order=6)
        ResponseStatus?: ResponseStatus;

        // @DataMember(Order=7)
        Meta?: { [index: string]: string; };
    }

    // @DataContract
    interface AssignRolesResponse {
        // @DataMember(Order=1)
        AllRoles?: string[];

        // @DataMember(Order=2)
        AllPermissions?: string[];

        // @DataMember(Order=3)
        ResponseStatus?: ResponseStatus;
    }

    // @DataContract
    interface UnAssignRolesResponse {
        // @DataMember(Order=1)
        AllRoles?: string[];

        // @DataMember(Order=2)
        AllPermissions?: string[];

        // @DataMember(Order=3)
        ResponseStatus?: ResponseStatus;
    }

    // @Route("/hello/{Name}")
    interface Hello extends IReturn<HelloResponse> {
        Name?: string;
    }

    /**
    * URL Shortening
    */
    // @Route("/shorten", "POST")
    // @Api("URL Shortening")
    interface ShortUrlRequest extends IReturn<ShortUrlResponse> {
        // @Required()
        // @ApiMember(Description="The destination for the new short URL.", IsRequired=true)
        Url: string;

        // @ApiMember(Description="If provided, creates short URL with given custom path. Throws error if path is already used.")
        CustomPath?: string;
    }

    // @Route("/{Key}", "GET")
    interface FollowShortUrlRequest {
        Key?: string;
    }

    // @Route("/auth")
    // @Route("/auth/{provider}")
    // @Route("/authenticate")
    // @Route("/authenticate/{provider}")
    // @DataContract
    interface Authenticate extends IReturn<AuthenticateResponse> {
        // @DataMember(Order=1)
        provider?: string;

        // @DataMember(Order=2)
        State?: string;

        // @DataMember(Order=3)
        oauth_token?: string;

        // @DataMember(Order=4)
        oauth_verifier?: string;

        // @DataMember(Order=5)
        UserName?: string;

        // @DataMember(Order=6)
        Password?: string;

        // @DataMember(Order=7)
        RememberMe?: boolean;

        // @DataMember(Order=8)
        Continue?: string;

        // @DataMember(Order=9)
        nonce?: string;

        // @DataMember(Order=10)
        uri?: string;

        // @DataMember(Order=11)
        response?: string;

        // @DataMember(Order=12)
        qop?: string;

        // @DataMember(Order=13)
        nc?: string;

        // @DataMember(Order=14)
        cnonce?: string;

        // @DataMember(Order=15)
        Meta?: { [index: string]: string; };
    }

    // @Route("/assignroles")
    // @DataContract
    interface AssignRoles extends IReturn<AssignRolesResponse> {
        // @DataMember(Order=1)
        UserName?: string;

        // @DataMember(Order=2)
        Permissions?: string[];

        // @DataMember(Order=3)
        Roles?: string[];
    }

    // @Route("/unassignroles")
    // @DataContract
    interface UnAssignRoles extends IReturn<UnAssignRolesResponse> {
        // @DataMember(Order=1)
        UserName?: string;

        // @DataMember(Order=2)
        Permissions?: string[];

        // @DataMember(Order=3)
        Roles?: string[];
    }

}
 