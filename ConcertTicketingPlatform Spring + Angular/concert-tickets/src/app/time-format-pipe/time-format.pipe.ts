import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeFormat',
  standalone: true
})
export class TimeFormatPipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return value;
    const [hours, minutes] = value.split(':');
    return `${hours}:${minutes}`;
  }
}