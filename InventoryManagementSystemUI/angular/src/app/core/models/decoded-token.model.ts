// src/app/core/models/decoded-token.model.ts
export interface DecodedToken {
  userId: string;
  name: string;
  exp: number;
  roleId: string;
  [key: string]: any; // For any additional claims
}
