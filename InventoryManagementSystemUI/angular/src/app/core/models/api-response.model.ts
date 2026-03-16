export interface ApiResponse<T> {
    Status: boolean;
    Message: string;
    Data: T;
    HttpStatus: string;
  }
  