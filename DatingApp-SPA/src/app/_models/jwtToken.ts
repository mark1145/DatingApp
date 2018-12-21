export interface JwtToken {
    nameid: number;
    unique_name: string;
    nbf: number;
    exp: number;
    iat: number;
}
