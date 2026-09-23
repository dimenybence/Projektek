package hu.bme.aut.android.todoapp

import android.graphics.Color
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.github.mikephil.charting.data.PieData
import com.github.mikephil.charting.data.PieDataSet
import com.github.mikephil.charting.data.PieEntry
import hu.bme.aut.android.todoapp.data.TodoItemDatabase
import hu.bme.aut.android.todoapp.databinding.ActivityChartBinding
import kotlin.concurrent.thread

class ChartActivity : AppCompatActivity() {
    private lateinit var binding: ActivityChartBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityChartBinding.inflate(layoutInflater)
        setContentView(binding.root)

        setupChart()
    }

    private fun setupChart() {
        val pieChart = binding.chartHoliday

        thread {
            val todoItemDao = TodoItemDatabase.getDatabase(this).todoItemDao()
            val completedItems = todoItemDao.getItemByIsCompleted(true)
            val incompleteItems = todoItemDao.getItemByIsCompleted(false)

            val totalTodos = completedItems.size + incompleteItems.size
            val completedTodos = completedItems.size.toFloat()
            val incompleteTodos = totalTodos - completedTodos

            val entries = ArrayList<PieEntry>()
            entries.add(PieEntry(completedTodos, "Completed"))
            entries.add(PieEntry(incompleteTodos, "Incomplete"))

            val dataSet = PieDataSet(entries, "Todo Status")
            dataSet.colors = listOf(Color.GREEN, Color.RED)

            val data = PieData(dataSet)

            runOnUiThread {
                pieChart.data = data
                pieChart.description.isEnabled = false
                pieChart.legend.isEnabled = false
                pieChart.setEntryLabelColor(Color.BLACK)
                pieChart.setHoleColor(Color.TRANSPARENT)
                pieChart.animateY(1000)
                pieChart.invalidate()
            }
        }
    }

}
