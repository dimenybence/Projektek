import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Purchase } from '../classes/purchase';

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private apiUrl = 'http://localhost:8081/api/purchase';

  constructor(private http: HttpClient) { }

  createPurchase(purchaseData: any): Observable<Purchase> {
    return this.http.post<Purchase>(this.apiUrl, purchaseData);
  }

  getUserPurchases(userId: number): Observable<Purchase[]> {
    return this.http.get<Purchase[]>(`${this.apiUrl}/${userId}`);
  }

  getAllPurchases(): Observable<Purchase[]> {
    return this.http.get<Purchase[]>(this.apiUrl);
  }
}