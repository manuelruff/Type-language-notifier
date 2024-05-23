import java.awt.event.*;
import java.io.File;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.swing.*;

class Test {
    // Sound file locations
    private static final File hebrew = new File("sounds\\Hebrew.wav");
    private static final File english = new File("sounds\\English.wav");

    public static void main(String[] args) {
        System.out.println("Hello World");

        // Create and show GUI
        SwingUtilities.invokeLater(() -> {
            createAndShowGUI();
        });
    }

    /**
     * Plays a sound from the specified file location.
     *
     * @param language the file location of the sound to be played
     */
    public static void playSound(File language) {
        try {
            AudioInputStream audioInputStream = AudioSystem.getAudioInputStream(language);
            Clip clip = AudioSystem.getClip();
            clip.open(audioInputStream);
            clip.start();
        } catch (Exception e) {
            System.out.println("Error with playing sound.");
        }
    }

    private static void createAndShowGUI() {
        JFrame frame = new JFrame("Key Press Listener");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        JTextField textField = new JTextField(20);
        textField.addKeyListener(new MyKeyListener());

        frame.getContentPane().add(textField);
        frame.pack();
        frame.setVisible(true);
    }

    static class MyKeyListener implements KeyListener {
        public void keyPressed(KeyEvent e) {
            playSound(hebrew); // Play sound when any key is pressed
        }

        public void keyReleased(KeyEvent e) {
            // Not needed for this example
        }

        public void keyTyped(KeyEvent e) {
            // Not needed for this example
        }
    }
}
