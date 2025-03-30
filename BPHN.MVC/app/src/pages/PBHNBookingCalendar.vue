<template>
  <div class="calendar-container">
    <div class="calendar">
      <h2>{{ currentMonth }} - Selected: {{ selectedRangeText }}</h2>
      <div class="calendar-grid">
        <div 
          v-for="day in days" 
          :key="day.date" 
          class="calendar-day" 
          :class="{ 'selected-week': isInSelectedWeek(day.date) }"
          @click="selectWeek(day.date)">
          <span>{{ day.date }}</span>
          <div class="event-markers">
            <span :style="{ background: getDayStatusColor(day.date) }" class="event-dot"></span>
          </div>
        </div>
      </div>
    </div>
    <div class="event-sidebar" v-if="selectedWeek.length">
      <h3>Facilities & Fields</h3>
      <ul>
        <li v-for="facility in facilities" :key="facility.id">
          <strong>{{ facility.name }}</strong>
          <ul>
            <li v-for="field in facility.fields" :key="field.id">
              {{ field.name }} - {{ field.date }} - {{ field.time }}
              <span :class="{ 'booked': field.booked }">{{ field.booked ? 'Booked' : 'Available' }}</span>
            </li>
          </ul>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      months: [
        'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'
      ],
      currentMonth: 'July',
      days: [],
      selectedWeek: [],
      facilities: []
    };
  },
  computed: {
    selectedRangeText() {
      if (this.selectedWeek.length) {
        return `${this.selectedWeek[0].date} - ${this.selectedWeek[this.selectedWeek.length - 1].date}`;
      }
      return 'None';
    }
  },
  methods: {
    setMonth(month) {
      this.currentMonth = month;
      this.generateDays();
    },
    generateDays() {
      this.days = Array.from({ length: 31 }, (_, i) => ({ 
        date: i + 1,
      }));
    },
    selectWeek(date) {
      const startOfWeek = Math.floor((date - 1) / 7) * 7 + 1;
      this.selectedWeek = this.days.filter(day => day.date >= startOfWeek && day.date < startOfWeek + 7);
      this.fetchFacilities();
    },
    isInSelectedWeek(date) {
      return this.selectedWeek.some(day => day.date === date);
    },
    fetchFacilities() {
      // Fake data for facilities and fields
      this.facilities = [
        {
          id: 1,
          name: 'Sports Center A',
          fields: [
            { id: 101, name: 'Field 1', date: 10, time: '10:00 AM', booked: false },
            { id: 102, name: 'Field 2', date: 11, time: '2:00 PM', booked: true }
          ]
        },
        {
          id: 2,
          name: 'Sports Center B',
          fields: [
            { id: 201, name: 'Field 3', date: 12, time: '6:00 PM', booked: false },
            { id: 202, name: 'Field 4', date: 13, time: '4:00 PM', booked: true }
          ]
        }
      ];
    },
    getDayStatusColor(date) {
      const allFields = this.facilities.flatMap(facility => facility.fields.filter(field => field.date === date));
      if (allFields.length === 0) return 'transparent';
      const bookedCount = allFields.filter(field => field.booked).length;
      if (bookedCount === 0) return 'green';
      if (bookedCount === allFields.length) return 'red';
      return 'yellow';
    }
  },
  mounted() {
    this.generateDays();
  }
};
</script>

<style scoped>
.calendar-container {
  display: flex;
  font-family: Arial, sans-serif;
}
.calendar {
  flex: 1;
  padding: 20px;
  background: #f5f5f5;
}
.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 10px;
}
.calendar-day {
  background: white;
  padding: 20px;
  text-align: center;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  cursor: pointer;
}
.selected-week {
  background: #d3e2ff;
}
.event-markers {
  margin-top: 5px;
  display: flex;
  justify-content: center;
  gap: 5px;
}
.event-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
}
.event-sidebar {
  width: 300px;
  background: white;
  padding: 20px;
  border-left: 1px solid #ddd;
}
.event-sidebar h3 {
  margin-bottom: 10px;
}
.event-sidebar ul {
  list-style: none;
  padding: 0;
}
.event-sidebar li {
  padding: 5px 0;
}
.booked {
  color: red;
  font-weight: bold;
}
</style>
