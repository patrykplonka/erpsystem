import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


interface WarehouseItemDto {
  id: number;
  name: string;
  code: string;
  quantity: number;
  price: number;
  category: string;
}

interface CreateWarehouseItemDto {
  name: string;
  code: string;
  quantity: number;
  price: number;
  category: string;
}

@Component({
  selector: 'app-warehouse',
  standalone: true, 
  imports: [CommonModule, FormsModule], 
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css']
})
export class WarehouseComponent implements OnInit {
  warehouseItems: WarehouseItemDto[] = [];
  newItem: CreateWarehouseItemDto = {
    name: '',
    code: '',
    quantity: 0,
    price: 0,
    category: ''
  };

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadItems();
  }

  loadItems() {
    this.http.get<WarehouseItemDto[]>('https://localhost:7224/api/warehouse').subscribe(
      data => this.warehouseItems = data,
      error => console.error('Error loading warehouse items', error.status, error.message)
    );
  }

  addItem() {
    this.http.post<WarehouseItemDto>('https://localhost:7224/api/warehouse', this.newItem).subscribe(
      () => {
        this.loadItems();
        this.newItem = { name: '', code: '', quantity: 0, price: 0, category: '' };
      },
      error => console.error('Error adding item', error.status, error.message)
    );
  }
}
