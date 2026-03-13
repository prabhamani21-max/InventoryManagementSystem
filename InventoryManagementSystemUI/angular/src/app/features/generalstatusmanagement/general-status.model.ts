export interface GeneralStatus {
    id: number;
    name: string;
    isActive: boolean;
    createdDate?: Date;
    createdBy?: number;
    updatedBy?: number;
    updatedDate?: Date;
}