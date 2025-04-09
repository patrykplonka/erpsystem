import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WarehouseMovementsService {
  private apiUrl = 'https://localhost:7224/api/warehousemovements';

  constructor(private http: HttpClient) { }

  createMovement(movement: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, movement);
  }

  getMovementsByItem(warehouseItemId: number): Observable<any[]> {
    const url = `${this.apiUrl}/item/${warehouseItemId}`;
    return this.http.get<any[]>(url);
  }

  getAllMovements(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}
