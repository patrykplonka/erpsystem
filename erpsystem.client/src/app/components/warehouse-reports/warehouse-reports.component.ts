import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-warehouse-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './warehouse-reports.component.html',
  styleUrls: ['./warehouse-reports.component.css']
})
export class WarehouseReportsComponent implements OnInit {
  warehouseItems: WarehouseItemDto[] = [];
  currentUserEmail: string | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  operationLogs: OperationLog[] = [];
  showHistory: boolean = false;
  historyUserFilter: string = '';
  historyStartDateFilter: string = '';
  historyEndDateFilter: string = '';
  historyItemFilter: string = '';
  historySortField: string = '';
  historySortDirection: 'asc' | 'desc' = 'asc';
  currentReport: string | null = null;
  movementStartDate: string = '';
  movementEndDate: string = '';
  movementsInPeriod: WarehouseMovement[] = [];
  popularProducts: { name: string, issueCount: number }[] = [];
  isWarehouseOpen: boolean = false;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadItems();
    this.loadOperationLogs();
    this.currentUserEmail = this.authService.getCurrentUserEmail();
  }

  setReport(report: string) {
    this.currentReport = report;
    if (report === 'popular') {
      this.loadPopularProducts();
    }
  }

  formatDate(date: string | Date): string {
    const d = new Date(date);
    return d.toLocaleString('pl-PL', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  loadOperationLogs() {
    this.http
      .get<OperationLog[]>('https://localhost:7224/api/warehouse/operation-logs')
      .subscribe(
        (data) => (this.operationLogs = data),
        (error) =>
          (this.errorMessage = `Błąd ładowania historii: ${error.status} ${error.message}`)
      );
  }

  applyHistoryFilters() {
    this.sortHistory(this.historySortField);
  }

  get filteredOperationLogs(): OperationLog[] {
    let filtered = this.operationLogs.filter((log) => {
      const matchesUser =
        !this.historyUserFilter ||
        log.user.toLowerCase().includes(this.historyUserFilter.toLowerCase());
      const matchesStartDate =
        !this.historyStartDateFilter ||
        new Date(log.timestamp).getTime() >=
        new Date(this.historyStartDateFilter).getTime();
      const matchesEndDate =
        !this.historyEndDateFilter ||
        new Date(log.timestamp).getTime() <=
        new Date(this.historyEndDateFilter).getTime();
      const matchesItem =
        !this.historyItemFilter ||
        log.itemName.toLowerCase().includes(this.historyItemFilter.toLowerCase()) ||
        log.itemId.toString().includes(this.historyItemFilter);
      return matchesUser && matchesStartDate && matchesEndDate && matchesItem;
    });

    if (this.historySortField) {
      filtered.sort((a, b) => {
        const valueA = a[this.historySortField as keyof OperationLog];
        const valueB = b[this.historySortField as keyof OperationLog];
        if (this.historySortField === 'timestamp') {
          return this.historySortDirection === 'asc'
            ? new Date(valueA as string).getTime() - new Date(valueB as string).getTime()
            : new Date(valueB as string).getTime() - new Date(valueA as string).getTime();
        } else if (typeof valueA === 'string' && typeof valueB === 'string') {
          return this.historySortDirection === 'asc'
            ? valueA.localeCompare(valueB)
            : valueB.localeCompare(valueA);
        }
        return 0;
      });
    }

    return filtered;
  }

  loadItems() {
    this.http.get<WarehouseItemDto[]>('https://localhost:7224/api/warehouse').subscribe(
      (data) => {
        this.warehouseItems = data;
      },
      (error) =>
        (this.errorMessage = `Błąd ładowania produktów: ${error.status} ${error.message}`)
    );
  }

  toggleHistoryView() {
    this.showHistory = !this.showHistory;
    if (this.showHistory) {
      this.loadOperationLogs();
    }
  }

  exportToExcel() {
    const worksheet = XLSX.utils.json_to_sheet(
      this.filteredOperationLogs.map((log) => ({
        ID: log.id,
        Użytkownik: log.user,
        Operacja: log.operation,
        Produkt: log.itemName,
        'ID Produktu': log.itemId,
        Data: this.formatDate(log.timestamp),
        Szczegóły: log.details
      }))
    );
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Operation Logs');
    XLSX.writeFile(workbook, 'warehouse_operation_logs.xlsx');
  }

  sortHistory(field: string) {
    if (this.historySortField === field) {
      this.historySortDirection = this.historySortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.historySortField = field;
      this.historySortDirection = 'asc';
    }
    this.applyHistoryFilters();
  }

  loadMovementsInPeriod() {
    const start = this.movementStartDate;
    const end = this.movementEndDate;
    if (!start || !end) {
      this.errorMessage = 'Obie daty są wymagane.';
      return;
    }
    this.http
      .get<WarehouseMovement[]>(
        `https://localhost:7224/api/warehousemovements/period?start=${start}&end=${end}`
      )
      .subscribe(
        (data) => {
          this.movementsInPeriod = data.map((movement) => ({
            ...movement,
            warehouseItemName:
              this.warehouseItems.find((item) => item.id === movement.warehouseItemId)?.name ||
              'Nieznany'
          }));
          this.successMessage = 'Załadowano ruchy w okresie.';
          this.errorMessage = null;
        },
        (error) =>
          (this.errorMessage = `Błąd ładowania ruchów: ${error.status} ${error.message}`)
      );
  }

  loadPopularProducts() {
    this.http
      .get<WarehouseMovement[]>('https://localhost:7224/api/warehousemovements')
      .subscribe(
        (movements) => {
          const issueMovements = movements.filter((m) => m.movementType === 'Wydanie');
          const productIssueCount = issueMovements.reduce(
            (acc, m) => {
              acc[m.warehouseItemId] = (acc[m.warehouseItemId] || 0) + 1;
              return acc;
            },
            {} as Record<number, number>
          );

          this.popularProducts = this.warehouseItems
            .map((item) => ({
              name: item.name,
              issueCount: productIssueCount[item.id] || 0
            }))
            .sort((a, b) => b.issueCount - a.issueCount);
        },
        (error) =>
          (this.errorMessage = `Błąd ładowania ruchów: ${error.status} ${error.message}`)
      );
  }

  toggleWarehouseMenu() {
    this.isWarehouseOpen = !this.isWarehouseOpen;
  }

  goToProducts() {
    this.router.navigate(['/products']);
  }

  goToMovements() {
    this.router.navigate(['/movements']);
  }

  goToReports() {
    this.router.navigate(['/reports']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

interface WarehouseItemDto {
  id: number;
  name: string;
  code: string;
  quantity: number;
  price: number;
  category: string;
  location: string;
}

interface OperationLog {
  id: number;
  user: string;
  operation: string;
  itemId: number;
  itemName: string;
  timestamp: string;
  details: string;
}

interface WarehouseMovement {
  id: number;
  warehouseItemId: number;
  movementType: string;
  quantity: number;
  supplier: string;
  documentNumber: string;
  description: string;
  date: string;
  createdBy: string;
  status: 'Zaplanowane' | 'W trakcie' | 'Zakończone';
  comment?: string;
  warehouseItemName?: string;
}
